using System.ComponentModel.DataAnnotations;
namespace SubscriptionManager.Api.DTO.Users
{
    public class UserRegistrationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}