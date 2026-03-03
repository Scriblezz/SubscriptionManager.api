using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.DTO.Subscriptions;

namespace SubscriptionManager.Api.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionDTO>> GetAllAsync();

        Task<SubscriptionDTO> GetByIdAsync(int id);

        Task<SubscriptionDTO> CreateSubscriptionAsync(Subscription subscription);

        Task<SubscriptionDTO> UpdateSubscriptionAsync(int id, Subscription subscription);

        Task<SubscriptionDTO> DeleteSubscriptionAsync(int id);

        Task<SubscriptionDTO> RenewAsync(int id);
    }
}