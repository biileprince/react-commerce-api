using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Entities;
using ReactCommerce.Api.Features.Addresses.DTOs;
using ReactCommerce.Api.Features.Orders.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Orders;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private const decimal ShippingFee = 20m;
    private const decimal FreeShippingThreshold = 500m;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(OrderResponse? Response, string? Error)> CreateOrderAsync(Guid userId, CreateOrderRequest req)
    {
        var address = await _db.Addresses.FindAsync(req.AddressId);
        if (address is null) return (null, "Invalid shipping address");

        var productIds = req.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();

        if (products.Count != req.Items.Count)
            return (null, "One or more products not found");

        var orderItems = new List<OrderItem>();
        decimal subtotal = 0;

        foreach (var item in req.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);

            if (item.Quantity > product.StockQuantity)
                return (null, $"Insufficient stock for {product.Name}");

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

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return (ToDto(order, address), null);
    }

    public async Task<PaginatedResponse<OrderResponse>> GetOrdersAsync(Guid userId, int page = 1, int limit = 10)
    {
        var q = _db.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var total = await q.CountAsync();
        page = Math.Max(1, page);
        limit = Math.Clamp(limit, 1, 50);

        var orders = await q.Skip((page - 1) * limit).Take(limit).ToListAsync();

        return new PaginatedResponse<OrderResponse>
        {
            Data = orders.Select(o => ToDto(o, o.ShippingAddress)).ToList(),
            Meta = new PaginationMeta
            {
                Page = page,
                Limit = limit,
                Total = total,
                TotalPages = (int)Math.Ceiling((double)total / limit),
            }
        };
    }

    public async Task<OrderResponse?> GetOrderAsync(Guid userId, Guid orderId)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order is null) return null;

        return ToDto(order, order.ShippingAddress);
    }

    private static OrderResponse ToDto(Order o, Address a) => new(
        o.Id, o.Subtotal, o.ShippingFee, o.Total,
        o.Currency, o.Status, o.CreatedAt,
        new AddressResponse(a.Id, a.FullName, a.PhoneNumber, a.AddressLine1, a.AddressLine2,
            a.City, a.Region, a.District, a.Landmark, a.IsDefault),
        o.Items.Select(i => new OrderItemResponse(
            i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.ImageUrl)).ToList());
}
