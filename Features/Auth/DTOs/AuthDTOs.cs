namespace ReactCommerce.Api.Features.Auth.DTOs;

public record RegisterRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(UserDto User, string Token);
public record UserDto(Guid Id, string Name, string Email, string? Avatar, string Role, DateTime CreatedAt);
