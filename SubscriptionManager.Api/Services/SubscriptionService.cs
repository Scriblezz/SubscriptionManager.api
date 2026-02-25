using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Data;
using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.Exceptions;

namespace SubscriptionManager.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;

    public SubscriptionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions.AsNoTracking().ToListAsync();
    }

    public async Task<Subscription> GetByIdAsync(int id)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
    {
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<Subscription> UpdateSubscriptionAsync(int id, Subscription subscription)
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
        return existingSubscription;
    }

    public async Task<Subscription> DeleteSubscriptionAsync(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null)
        {
            return null;
        }
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<Subscription> RenewAsync(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        // Check if the subscription exists
        if (subscription == null)
        {
            throw new NotFoundException("Subscription not found");
        }
        // Check if the subscription is active and has a valid billing cycle
        if (!subscription.IsActive || subscription.BillingCycle == Unknown)
        {
            throw new BadRequestException("Subscription is not active");
        }
        // Check if the subscription can be renewed based on the next renewal date
        if (subscription.NextRenewalDate > DateTime.Now)
        {
            throw new BadRequestException("Subscription cannot be renewed yet");
        }

        // Update the subscription's renewal dates based on the billing cycle
        while (subscription.NextRenewalDate <= DateTime.Now)
        {
            if (subscription.BillingCycle == BillingCycle.Weekly)
            {
                subscription.LastRenewalDate = DateOnly.FromDateTime(DateTime.Now);
                subscription.NextRenewalDate = subscription.NextRenewalDate.AddDays(7);
            }
            else if (subscription.BillingCycle == BillingCycle.Yearly)
            {
                subscription.LastRenewalDate = DateOnly.FromDateTime(DateTime.Now);
                subscription.NextRenewalDate = subscription.NextRenewalDate.AddYears(1);
            }
            else if (subscription.BillingCycle == BillingCycle.Monthly)
            {
                subscription.LastRenewalDate = DateOnly.FromDateTime(DateTime.Now);
                subscription.NextRenewalDate = subscription.NextRenewalDate.AddMonths(1);
            }
        }
        await _context.SaveChangesAsync();
        return subscription;
    }
}