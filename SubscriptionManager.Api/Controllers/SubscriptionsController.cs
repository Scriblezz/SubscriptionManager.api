using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Data;
using SubscriptionManager.Api.Services;
using SubscriptionManager.Api.Entities;

namespace SubscriptionManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        // Inject the SubscriptionService into the controller
        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // GET: api/subscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetAllSubscriptions()
        {
            // Call the service to get all subscriptions
            var subscriptions = await _subscriptionService.GetAllAsync();
            // Return the subscriptions as the response
            return Ok(subscriptions);
        }

        // GET: api/subscriptions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscriptionById(int id)
        {
            // Call the service to get the subscription by ID
            var subscription = await _subscriptionService.GetByIdAsync(id);
            // If the subscription is not found, return a 404 Not Found response
            if (subscription == null)
            {
                return NotFound();
            }
            return Ok(subscription);
        }

        [HttpPost]
        public async Task<ActionResult<Subscription>> CreateSubscription(Subscription subscription, [FromServices] AppDbContext context)
        {
            // Call the service to create a new subscription
            context.Subscriptions.Add(subscription);
            await context.SaveChangesAsync();
            // Return the created subscription with a 201 Created response
            return CreatedAtAction(nameof(GetSubscriptionById), new { id = subscription.Id }, subscription);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubscription(int id, Subscription subscription, [FromServices] AppDbContext context)
        {
            // Call the service to update the subscription
            var existingSubscription = await context.Subscriptions.FindAsync(id);
            if (existingSubscription == null)
            {
                return NotFound();
            }
            existingSubscription.Name = subscription.Name;
            existingSubscription.Price = subscription.Price;
            existingSubscription.Category = subscription.Category;
            existingSubscription.IsActive = subscription.IsActive;
            existingSubscription.NextRenewalDate = subscription.NextRenewalDate;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(int id, [FromServices] AppDbContext context)
        {
            // Call the service to delete the subscription
            var subscription = await context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            context.Subscriptions.Remove(subscription);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}