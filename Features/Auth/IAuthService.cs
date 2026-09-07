using System.Security.Claims;
using ReactCommerce.Api.Features.Auth.DTOs;

namespace ReactCommerce.Api.Features.Auth;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> GetMeAsync(ClaimsPrincipal claims);
    Task<bool> CheckEmailExistsAsync(string email);
}
