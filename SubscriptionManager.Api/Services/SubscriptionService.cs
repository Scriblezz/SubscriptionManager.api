using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Data;
using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.Exceptions;
using SubscriptionManager.Api.DTO.Subscriptions;

namespace SubscriptionManager.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;

    public SubscriptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriptionDTO>> GetAllAsync()
    {
        var subs = await _context.Subscriptions.AsNoTracking().ToListAsync();
        return subs.Select(ToResponse).ToList();
    }

    public async Task<SubscriptionDTO> GetByIdAsync(int id)
    {
        var sub = await _context.Subscriptions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
        return ToResponse(sub);
    }

    public async Task<SubscriptionDTO> CreateSubscriptionAsync(Subscription subscription)
    {
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return ToResponse(subscription);
    }

    public async Task<SubscriptionDTO> UpdateSubscriptionAsync(int id, Subscription subscription)
    {
        var existingSubscription = await _context.Subscriptions.FindAsync(id);
        if (existingSubscription == null)
        {
            return null;
        }
        existingSubscription.Name = subscription.Name;
        existingSubscription.Price = subscription.Price;
        existingSubscription.Category = subscription.Category;
        existingSubscription.IsActive = subscription.IsActive;
        existingSubscription.NextRenewalDate = subscription.NextRenewalDate;
        existingSubscription.LastRenewalDate = subscription.LastRenewalDate;
        existingSubscription.BillingCycle = subscription.BillingCycle;
        await _context.SaveChangesAsync();
        return ToResponse(existingSubscription);
    }

    public async Task<SubscriptionDTO> DeleteSubscriptionAsync(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null)
        {
            return null;
        }
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return ToResponse(subscription);
    }

    public async Task<SubscriptionDTO> RenewAsync(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        // Check if the subscription exists
        if (subscription == null)
        {
            throw new NotFoundException("Subscription not found");
        }
        // Check if the subscription is active
        if (!subscription.IsActive)
            throw new BadRequestException("Subscription is not active");
        // Check if the billing cycle is valid
        if (subscription.BillingCycle == BillingCycle.Unknown)
            throw new BadRequestException("Subscription billing cycle is invalid");
        // Check if the subscription can be renewed based on the next renewal date
        if (subscription.NextRenewalDate > DateTime.UtcNow)
        {
            throw new BadRequestException("Subscription cannot be renewed yet");
        }

        // Update the subscription's renewal dates based on the billing cycle
        while (subscription.NextRenewalDate <= DateTime.UtcNow)
        {
            if (subscription.BillingCycle == BillingCycle.Weekly)
            {
                subscription.LastRenewalDate = DateOnly.FromDateTime(DateTime.UtcNow);
                subscription.NextRenewalDate = subscription.NextRenewalDate.AddDays(7);
            }
            else if (subscription.BillingCycle == BillingCycle.Yearly)
            {
                subscription.LastRenewalDate = DateOnly.FromDateTime(DateTime.UtcNow);
                subscription.NextRenewalDate = subscription.NextRenewalDate.AddYears(1);
            }
            else if (subscription.BillingCycle == BillingCycle.Monthly)
            {
                subscription.LastRenewalDate = DateOnly.FromDateTime(DateTime.UtcNow);
                subscription.NextRenewalDate = subscription.NextRenewalDate.AddMonths(1);
            }
        }
        await _context.SaveChangesAsync();
        return ToResponse(subscription);
    }

    private static SubscriptionDTO ToResponse(Subscription s)
    {
        if (s == null) return null;

        return new SubscriptionDTO
        {
            Id = s.Id,
            Name = s.Name,
            Price = s.Price,
            Category = s.Category,
            IsActive = s.IsActive,
            NextRenewalDate = s.NextRenewalDate,
            LastRenewalDate = s.LastRenewalDate,
            BillingCycle = s.BillingCycle
        };
    }
}