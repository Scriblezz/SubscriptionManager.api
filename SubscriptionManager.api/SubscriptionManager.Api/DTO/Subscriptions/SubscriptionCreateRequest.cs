using System.ComponentModel.DataAnnotations;
using SubscriptionManager.Api.Entities;

namespace SubscriptionManager.Api.DTO.Subscriptions;

public class SubscriptionCreateRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public SubscriptionManager.Api.Entities.BillingCycle? BillingCycle { get; set; }
}