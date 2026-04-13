using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Data;
using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.Exceptions;
using SubscriptionManager.Api.DTO.Subscriptions;
using SubscriptionManager.Api.DTOs.Common;
using System.Security.Claims;

namespace SubscriptionManager.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SubscriptionService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResponse<SubscriptionDTO>> GetAllAsync(string? category, bool? isActive, string? sortBy, string? sortDirection, int page, int pageSize)
    {
        var currentUserId = GetCurrentUserId();
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var query = _context.Subscriptions.AsQueryable();
        query = query.Where(s => s.UserId == currentUserId);

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(s => s.Category == category);
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        // Sorting defaults
        var normalizedSortBy = sortBy?.Trim().ToLower();
        var normalizedSortDirection = sortDirection?.Trim().ToLower() ?? "asc";

        if (normalizedSortDirection != "asc" && normalizedSortDirection != "desc")
        {
            throw new BadRequestException("sortDirection must be either 'asc' or 'desc'.");
        }

        var isDescending = normalizedSortDirection == "desc";

        // Sorting
        switch (normalizedSortBy)
        {
            case null:
            case "":
                query = isDescending
                    ? query.OrderByDescending(s => s.Id)
                    : query.OrderBy(s => s.Id);
                break;

            case "name":
                query = isDescending
                    ? query.OrderByDescending(s => s.Name)
                    : query.OrderBy(s => s.Name);
                break;

            case "price":
                query = isDescending
                    ? query.OrderByDescending(s => s.Price)
                    : query.OrderBy(s => s.Price);
                break;

            case "nextrenewaldate":
                query = isDescending
                    ? query.OrderByDescending(s => s.NextRenewalDate)
                    : query.OrderBy(s => s.NextRenewalDate);
                break;

            default:
                throw new BadRequestException(
                    "Invalid sortBy value. Allowed values are: name, price, nextRenewalDate, createdAt.");
        }


        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var skip = (page - 1) * pageSize;

        var subs = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<SubscriptionDTO>
        {
            Items = subs.Select(ToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<SubscriptionDTO> GetByIdAsync(int id)
    {
        var currentUserId = GetCurrentUserId();
        var sub = await _context.Subscriptions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == currentUserId);
        if (sub == null)
        {
            throw new NotFoundException($"Subscription {id} not found");
        }
        return ToResponse(sub);
    }

    public async Task<SubscriptionDTO> CreateSubscriptionAsync(SubscriptionCreateRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (request.BillingCycle!.Value == BillingCycle.Unknown)
        {
            throw new BadRequestException("Invalid billing cycle");
        }
        var now = DateTime.UtcNow;

        var newSubscription = new Subscription
        {
            Name = request.Name,
            UserId = currentUserId,
            Price = request.Price,
            Category = request.Category,
            BillingCycle = request.BillingCycle.Value,

            IsActive = true,
            LastRenewalDate = null,
            NextRenewalDate = GetInitialNextRenewalDate(now, request.BillingCycle.Value)
        };

        _context.Subscriptions.Add(newSubscription);
        await _context.SaveChangesAsync();

        return ToResponse(newSubscription);
    }

    public async Task<SubscriptionDTO> UpdateSubscriptionAsync(int id, SubscriptionUpdateRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var existingSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == currentUserId);
        if (existingSubscription == null)
        {
            throw new NotFoundException($"Subscription {id} not found");
        }

        if (request.BillingCycle!.Value == BillingCycle.Unknown)
        {
            throw new BadRequestException("Invalid billing cycle");
        }

        existingSubscription.Name = request.Name;
        existingSubscription.Price = request.Price;
        existingSubscription.Category = request.Category;
        existingSubscription.IsActive = request.IsActive;
        existingSubscription.BillingCycle = request.BillingCycle.Value;

        await _context.SaveChangesAsync();

        return ToResponse(existingSubscription);
    }

    public async Task<SubscriptionDTO> DeleteSubscriptionAsync(int id)
    {
        var currentUserId = GetCurrentUserId();
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == currentUserId);
        if (subscription == null)
        {
            throw new NotFoundException($"Subscription {id} not found");
        }
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return ToResponse(subscription);
    }

    public async Task<SubscriptionDTO> RenewAsync(int id)
    {
        var currentUserId = GetCurrentUserId();
        var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id && s.UserId == currentUserId);
        // Check if the subscription exists
        if (subscription == null)
        {
            throw new NotFoundException("Subscription not found");
        }
        // Check if the subscription is active
        if (!subscription.IsActive)
            throw new BadRequestException("Subscription is not active");
        // Check if the billing cycle is valid
        if (subscription.BillingCycle == BillingCycle.Unknown)
            throw new BadRequestException("Subscription billing cycle is invalid");
        // Check if the subscription can be renewed based on the next renewal date
        if (subscription.NextRenewalDate > DateTime.UtcNow)
        {
            throw new BadRequestException("Subscription cannot be renewed yet");
        }

        // Update the subscription's renewal dates based on the billing cycle
        while (subscription.NextRenewalDate <= DateTime.UtcNow)
        {
            var last = subscription.NextRenewalDate;

            subscription.NextRenewalDate = subscription.BillingCycle switch
            {
                BillingCycle.Weekly => subscription.NextRenewalDate.AddDays(7),
                BillingCycle.Monthly => subscription.NextRenewalDate.AddMonths(1),
                BillingCycle.Yearly => subscription.NextRenewalDate.AddYears(1),
                _ => throw new BadRequestException("Subscription billing cycle is invalid")
            };

            subscription.LastRenewalDate = DateOnly.FromDateTime(last);
        }
        await _context.SaveChangesAsync();
        return ToResponse(subscription);
    }

    private static SubscriptionDTO ToResponse(Subscription s)
    {

        return new SubscriptionDTO
        {
            Id = s.Id,
            Name = s.Name,
            Price = s.Price,
            Category = s.Category,
            IsActive = s.IsActive,
            NextRenewalDate = s.NextRenewalDate,
            LastRenewalDate = s.LastRenewalDate,
            BillingCycle = s.BillingCycle
        };
    }

    private static DateTime GetInitialNextRenewalDate(DateTime now, BillingCycle billingCycle)
    {
        return billingCycle switch
        {
            BillingCycle.Weekly => now.AddDays(7),
            BillingCycle.Monthly => now.AddMonths(1),
            BillingCycle.Yearly => now.AddYears(1),
            _ => throw new BadRequestException("Invalid billing cycle")
        };
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
        }
        return userId;
    }
}