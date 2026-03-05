namespace SubscriptionManager.Api.DTO.Subscriptions;

public class SubscriptionDTO
{

	public int Id { get; set; }
	public string Name { get; set; }
	public decimal Price { get; set; }
	public string Category { get; set; }
	public bool IsActive { get; set; }
	public DateTime NextRenewalDate { get; set; }
	public DateOnly? LastRenewalDate { get; set; }
	public SubscriptionManager.Api.Entities.BillingCycle BillingCycle { get; set; }
}
