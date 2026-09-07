namespace ReactCommerce.Api.Features.Categories.DTOs;

public record CategoryResponse(Guid Id, string Name, string Slug, string Description, string? Icon, bool IsActive, int ProductCount);
