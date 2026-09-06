using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Entities;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Auth;

public record RegisterRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(UserDto User, string Token);
public record UserDto(Guid Id, string Name, string Email, string? Avatar, string Role, DateTime CreatedAt);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapGet("/me", GetMe).RequireAuthorization();
    }

    private static async Task<IResult> Register(RegisterRequest req, AppDbContext db, IConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || req.Name.Length < 2)
            return Results.BadRequest(ApiResponse<object>.Fail("Name must be at least 2 characters"));

        if (string.IsNullOrWhiteSpace(req.Email))
            return Results.BadRequest(ApiResponse<object>.Fail("Email is required"));

        if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 8)
            return Results.BadRequest(ApiResponse<object>.Fail("Password must be at least 8 characters"));

        var exists = await db.Users.AnyAsync(u => u.Email == req.Email.ToLower().Trim());
        if (exists)
            return Results.Conflict(ApiResponse<object>.Fail("An account with this email already exists."));

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Email = req.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = "customer",
            CreatedAt = DateTime.UtcNow,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var token = GenerateToken(user, config);
        var dto = ToDto(user);

        return Results.Created($"/api/auth/me", ApiResponse<AuthResponse>.Ok(new AuthResponse(dto, token)));
    }

    private static async Task<IResult> Login(LoginRequest req, AppDbContext db, IConfiguration config)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower().Trim());
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Results.Unauthorized();

        var token = GenerateToken(user, config);
        var dto = ToDto(user);

        return Results.Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse(dto, token)));
    }

    private static async Task<IResult> GetMe(ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();
        var user = await db.Users.FindAsync(userId);
        if (user is null) return Results.NotFound();

        return Results.Ok(ApiResponse<UserDto>.Ok(ToDto(user)));
    }

    private static string GenerateToken(User user, IConfiguration config)
    {
        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto ToDto(User user) =>
        new(user.Id, user.Name, user.Email, user.Avatar, user.Role, user.CreatedAt);
}
