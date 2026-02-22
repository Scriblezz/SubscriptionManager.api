using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Data;
using SubscriptionManager.Api.Entities;

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
}