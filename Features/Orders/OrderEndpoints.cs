using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Entities;
using ReactCommerce.Api.Features.Addresses;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Orders;

public record CreateOrderRequest(Guid AddressId, List<OrderItemRequest> Items);
public record OrderItemRequest(Guid ProductId, int Quantity);

public record OrderResponse(
    Guid Id, decimal Subtotal, decimal ShippingFee, decimal Total,
    string Currency, string Status, DateTime CreatedAt,
    AddressResponse ShippingAddress, List<OrderItemResponse> Items);

public record OrderItemResponse(
    Guid ProductId, string ProductName, decimal UnitPrice,
    int Quantity, string? ImageUrl);

public static class OrderEndpoints
{
    private const decimal ShippingFee = 20m;
    private const decimal FreeShippingThreshold = 500m;

    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapPost("/", CreateOrder);
        group.MapGet("/", GetOrders).RequireAuthorization();
        group.MapGet("/{id:guid}", GetOrder).RequireAuthorization();
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderRequest req, ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.GetUserId();

        if (req.Items is null || req.Items.Count == 0)
            return Results.BadRequest(ApiResponse<object>.Fail("Order must contain at least one item"));

        var address = await db.Addresses.FindAsync(req.AddressId);
        if (address is null)
            return Results.BadRequest(ApiResponse<object>.Fail("Invalid shipping address"));

        var productIds = req.Items.Select(i => i.ProductId).ToList();
        var products = await db.Products
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != req.Items.Count)
            return Results.BadRequest(ApiResponse<object>.Fail("One or more products not found"));

        var orderItems = new List<OrderItem>();
        decimal subtotal = 0;

        foreach (var item in req.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);

            if (item.Quantity <= 0 || item.Quantity > product.StockQuantity)
                return Results.BadRequest(ApiResponse<object>.Fail($"Invalid quantity for {product.Name}"));

            product.StockQuantity -= item.Quantity;

            var lineTotal = product.Price * item.Quantity;
            subtotal += lineTotal;

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                ImageUrl = product.Images.FirstOrDefault()?.Url,
            });
        }

        var shippingFee = subtotal >= FreeShippingThreshold ? 0 : ShippingFee;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShippingAddressId = req.AddressId,
            Subtotal = subtotal,
            ShippingFee = shippingFee,
            Total = subtotal + shippingFee,
            Status = "pending",
            Items = orderItems,
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var dto = ToDto(order, address);
        return Results.Created($"/api/orders/{order.Id}", ApiResponse<OrderResponse>.Ok(dto));
    }

    private static async Task<IResult> GetOrders(ClaimsPrincipal claims, AppDbContext db, int page = 1, int limit = 10)
    {
        var userId = claims.RequireUserId();

        var q = db.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var total = await q.CountAsync();
        page = Math.Max(1, page);
        limit = Math.Clamp(limit, 1, 50);

        var orders = await q.Skip((page - 1) * limit).Take(limit).ToListAsync();

        return Results.Ok(new PaginatedResponse<OrderResponse>
        {
            Data = orders.Select(o => ToDto(o, o.ShippingAddress)).ToList(),
            Meta = new PaginationMeta
            {
                Page = page,
                Limit = limit,
                Total = total,
                TotalPages = (int)Math.Ceiling((double)total / limit),
            }
        });
    }

    private static async Task<IResult> GetOrder(Guid id, ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();
        var order = await db.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order is null) return Results.NotFound();

        return Results.Ok(ApiResponse<OrderResponse>.Ok(ToDto(order, order.ShippingAddress)));
    }

    private static OrderResponse ToDto(Order o, Address a) => new(
        o.Id, o.Subtotal, o.ShippingFee, o.Total,
        o.Currency, o.Status, o.CreatedAt,
        new AddressResponse(a.Id, a.FullName, a.PhoneNumber, a.AddressLine1, a.AddressLine2,
            a.City, a.Region, a.District, a.Landmark, a.IsDefault),
        o.Items.Select(i => new OrderItemResponse(
            i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.ImageUrl)).ToList());
}
