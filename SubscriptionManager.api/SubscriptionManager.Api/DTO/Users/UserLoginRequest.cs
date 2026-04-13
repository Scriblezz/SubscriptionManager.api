using System.ComponentModel.DataAnnotations;

namespace SubscriptionManager.Api.DTO.Users
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}