using SubscriptionManager.Api.Entities;

namespace SubscriptionManager.Api.Services
{
    public interface ISubscriptionService
    {
        Task<List<Subscription>> GetAllAsync();

        Task<Subscription> GetByIdAsync(int id);

        Task<Subscription> CreateSubscriptionAsync(Subscription subscription);

        Task<Subscription> UpdateSubscriptionAsync(int id, Subscription subscription);

        Task<Subscription> DeleteSubscriptionAsync(int id);
    }
}