using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Api.Data;
using SubscriptionManager.Api.Entities;
using SubscriptionManager.Api.Exceptions;
using SubscriptionManager.Api.DTO.Users;

namespace SubscriptionManager.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<UserDTO> RegisterAsync(UserRegistrationRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new BadRequestException("Email is already in use.");
        }
        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email
        };
    }
}