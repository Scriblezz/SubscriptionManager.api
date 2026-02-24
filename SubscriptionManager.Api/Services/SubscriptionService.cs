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
        if (subscription == null)
        {
            throw new NotFoundException("Subscription not found");
        }
        if (!subscription.IsActive)
        {
            throw new BadRequestException("Subscription is not active");
        }
        
        subscription.LastRenewalDate = DateOnly.FromDateTime(DateTime.Now);
        subscription.NextRenewalDate = subscription.NextRenewalDate.AddMonths(1);
        await _context.SaveChangesAsync();
        return subscription;
    }
}