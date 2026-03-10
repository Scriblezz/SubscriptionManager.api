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

    public async Task<List<SubscriptionDTO>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var skip = (page - 1) * pageSize;

        var subs = await _context.Subscriptions
            .OrderBy(s => s.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return subs.Select(ToResponse).ToList();
    }

    public async Task<SubscriptionDTO> GetByIdAsync(int id)
    {
        var sub = await _context.Subscriptions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sub == null)
        {
            throw new NotFoundException($"Subscription {id} not found");
        }
        return ToResponse(sub);
    }

    public async Task<SubscriptionDTO> CreateSubscriptionAsync(SubscriptionCreateRequest request)
    {
        if (request.BillingCycle!.Value == BillingCycle.Unknown)
        {
            throw new BadRequestException("Invalid billing cycle");
        }
        var now = DateTime.UtcNow;

        var newSubscription = new Subscription
        {
            Name = request.Name,
            Price = request.Price,
            Category = request.Category,
            BillingCycle = request.BillingCycle.Value,

            IsActive = true,
            LastRenewalDate = null,
            NextRenewalDate = GetInitialNextRenewalDate(now, request.BillingCycle.Value)
        };

        _context.Subscriptions.Add(newSubscription);
        await _context.SaveChangesAsync();

        return ToResponse(newSubscription);
    }

    public async Task<SubscriptionDTO> UpdateSubscriptionAsync(int id, SubscriptionUpdateRequest request)
    {
        var existingSubscription = await _context.Subscriptions.FindAsync(id);
        if (existingSubscription == null)
        {
            throw new NotFoundException($"Subscription {id} not found");
        }

        if (request.BillingCycle!.Value == BillingCycle.Unknown)
        {
            throw new BadRequestException("Invalid billing cycle");
        }

        existingSubscription.Name = request.Name;
        existingSubscription.Price = request.Price;
        existingSubscription.Category = request.Category;
        existingSubscription.IsActive = request.IsActive;
        existingSubscription.BillingCycle = request.BillingCycle.Value;

        await _context.SaveChangesAsync();

        return ToResponse(existingSubscription);
    }

    public async Task<SubscriptionDTO> DeleteSubscriptionAsync(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null)
        {
            throw new NotFoundException($"Subscription {id} not found");
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
            var last = subscription.NextRenewalDate;

            subscription.NextRenewalDate = subscription.BillingCycle switch
            {
                BillingCycle.Weekly => subscription.NextRenewalDate.AddDays(7),
                BillingCycle.Monthly => subscription.NextRenewalDate.AddMonths(1),
                BillingCycle.Yearly => subscription.NextRenewalDate.AddYears(1),
                _ => throw new BadRequestException("Subscription billing cycle is invalid")
            };

            subscription.LastRenewalDate = DateOnly.FromDateTime(last);
        }
        await _context.SaveChangesAsync();
        return ToResponse(subscription);
    }

    private static SubscriptionDTO ToResponse(Subscription s)
    {

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

    private static DateTime GetInitialNextRenewalDate(DateTime now, BillingCycle billingCycle)
    {
        return billingCycle switch
        {
            BillingCycle.Weekly => now.AddDays(7),
            BillingCycle.Monthly => now.AddMonths(1),
            BillingCycle.Yearly => now.AddYears(1),
            _ => throw new BadRequestException("Invalid billing cycle")
        };
    }
}