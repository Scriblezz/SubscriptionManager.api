namespace SubscriptionManager.Api.DTO.Subscriptions;

public class SubscriptionUpdateRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public bool IsActive { get; set; }
    public SubscriptionManager.Api.Entities.BillingCycle BillingCycle { get; set; }
}