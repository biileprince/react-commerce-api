using System.Security.Claims;

namespace ReactCommerce.Api.Shared;

public static class CurrentUser
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return claim is not null ? Guid.Parse(claim) : null;
    }

    public static Guid RequireUserId(this ClaimsPrincipal user)
    {
        return user.GetUserId() ?? throw new UnauthorizedAccessException("User not authenticated");
    }
}
