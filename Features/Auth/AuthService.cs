using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Entities;
using ReactCommerce.Api.Features.Auth.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email.ToLower().Trim());
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest req)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Email = req.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = "customer",
            CreatedAt = DateTime.UtcNow,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateToken(user, _config);
        var dto = ToDto(user);

        return new AuthResponse(dto, token);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower().Trim());
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return null;

        var token = GenerateToken(user, _config);
        var dto = ToDto(user);

        return new AuthResponse(dto, token);
    }

    public async Task<UserDto?> GetMeAsync(ClaimsPrincipal claims)
    {
        var userId = claims.RequireUserId();
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return null;

        return ToDto(user);
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
