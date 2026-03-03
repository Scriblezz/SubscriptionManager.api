using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Data;
using SubscriptionManager.Api.Services;
using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.DTO.Subscriptions;

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
        public async Task<ActionResult<IEnumerable<SubscriptionDTO>>> GetAllSubscriptions()
        {
            // Call the service to get all subscriptions
            var subscriptions = await _subscriptionService.GetAllAsync();
            // Return the subscriptions as the response
            return Ok(subscriptions);
        }

        // GET: api/subscriptions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionDTO>> GetSubscriptionById(int id)
        {
            // Call the service to get the subscription by ID
            var subscription = await _subscriptionService.GetByIdAsync(id);
            if (subscription == null) return NotFound();
            return Ok(subscription);
        }

        [HttpPost]
        public async Task<ActionResult<SubscriptionDTO>> CreateSubscription(Subscription subscription)
        {
            // Call the service to create a new subscription
            var created = await _subscriptionService.CreateSubscriptionAsync(subscription);
            // Return the created subscription with a 201 Created response
            return CreatedAtAction(nameof(GetSubscriptionById), new { id = subscription.Id }, subscription);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubscription(int id, Subscription subscription)
        {
            // Call the service to update the subscription
            var updated = await _subscriptionService.UpdateSubscriptionAsync(id, subscription);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            // Call the service to delete the subscription
            var deleted = await _subscriptionService.DeleteSubscriptionAsync(id);
            if (deleted == null) return NotFound();
            return Ok(deleted);
        }

        [HttpPost("{id}/renew")]
        public async Task<IActionResult> RenewSubscription(int id)
        {
            // Call the service to renew the subscription
            var result = await _subscriptionService.RenewAsync(id);
            return Ok(result);
        }
    }
}