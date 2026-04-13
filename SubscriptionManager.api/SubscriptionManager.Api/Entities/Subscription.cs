namespace SubscriptionManager.Api.Entities;


public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime NextRenewalDate { get; set; }
        public DateOnly? LastRenewalDate { get; set; }
        public BillingCycle BillingCycle { get; set; }
    }

public enum BillingCycle
{
    Unknown,
    Weekly,
    Monthly,
    Yearly,
}