using SubscriptionManager.Api.DTO.Users;

namespace SubscriptionManager.Api.Services
{
	public interface IAuthService
	{
		Task<UserDTO> RegisterAsync(UserRegistrationRequest request);

        Task<UserLoginResponse> LoginAsync(UserLoginRequest request);
    }
}