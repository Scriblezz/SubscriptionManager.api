using SubscriptionManager.Api.Entities;

namespace SubscriptionManager.Api.Services
{
    public interface ISubscriptionService
    {
        Task<List<Subscription>> GetAllAsync();

        Task<Subscription> GetByIdAsync(int id);
    }
}