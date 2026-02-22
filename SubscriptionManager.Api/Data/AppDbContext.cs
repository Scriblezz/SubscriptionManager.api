using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Entities;

namespace SubscriptionManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}