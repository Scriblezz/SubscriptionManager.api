using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.DTOs.Common;
using SubscriptionManager.Api.DTO.Subscriptions;

namespace SubscriptionManager.Api.Services
{
    public interface ISubscriptionService
    {
        Task<PagedResponse<SubscriptionDTO>> GetAllAsync(string? category, bool? isActive, int page, int pageSize);

        Task<SubscriptionDTO> GetByIdAsync(int id);

        Task<SubscriptionDTO> CreateSubscriptionAsync(SubscriptionCreateRequest subscription);

        Task<SubscriptionDTO> UpdateSubscriptionAsync(int id, SubscriptionUpdateRequest subscription);

        Task<SubscriptionDTO> DeleteSubscriptionAsync(int id);

        Task<SubscriptionDTO> RenewAsync(int id);

    }
}