using System.ComponentModel.DataAnnotations;

namespace SubscriptionManager.Api.DTO.Users
{
    public class UserLoginResponse
    {
        public string Token { get; set; } = null!;
    }
}