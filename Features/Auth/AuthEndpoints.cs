using System.Security.Claims;
using ReactCommerce.Api.Features.Auth.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", Register)
             .AddEndpointFilter<ValidationFilter<RegisterRequest>>();
             
        group.MapPost("/login", Login)
             .AddEndpointFilter<ValidationFilter<LoginRequest>>();
             
        group.MapGet("/me", GetMe).RequireAuthorization();
    }

    private static async Task<IResult> Register(RegisterRequest req, IAuthService authService)
    {
        if (await authService.CheckEmailExistsAsync(req.Email))
            return Results.Conflict(ApiResponse<object>.Fail("An account with this email already exists."));

        var result = await authService.RegisterAsync(req);
        if (result is null) return Results.BadRequest();

        return Results.Created($"/api/auth/me", ApiResponse<AuthResponse>.Ok(result));
    }

    private static async Task<IResult> Login(LoginRequest req, IAuthService authService)
    {
        var result = await authService.LoginAsync(req);
        if (result is null) return Results.Unauthorized();

        return Results.Ok(ApiResponse<AuthResponse>.Ok(result));
    }

    private static async Task<IResult> GetMe(ClaimsPrincipal claims, IAuthService authService)
    {
        var result = await authService.GetMeAsync(claims);
        if (result is null) return Results.NotFound();

        return Results.Ok(ApiResponse<UserDto>.Ok(result));
    }
}
