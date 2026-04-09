using System.Net.Http.Json;

namespace SportsStore.CustomerPortal.Services;

public class OrderApiService
{
    private readonly HttpClient _http;

    public OrderApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        // Simula produtos enquanto nao ha endpoint de produtos
        return await Task.FromResult(new List<ProductDto>
        {
            new() { Id = "1", Name = "Football", Price = 25.99m, Category = "Sports" },
            new() { Id = "2", Name = "Tennis Racket", Price = 49.99m, Category = "Sports" },
            new() { Id = "3", Name = "Running Shoes", Price = 89.99m, Category = "Footwear" },
            new() { Id = "4", Name = "Yoga Mat", Price = 19.99m, Category = "Fitness" },
            new() { Id = "5", Name = "Basketball", Price = 35.99m, Category = "Sports" }
        });
    }

    public async Task<OrderDto?> CheckoutAsync(string customerId, List<CartItem> items)
    {
        var request = new
        {
            CustomerId = customerId,
            Items = items.Select(i => new
            {
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice
            })
        };

        var response = await _http.PostAsJsonAsync("api/orders/checkout", request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<OrderDto>();
        return null;
    }

    public async Task<List<OrderDto>> GetOrdersAsync(string customerId)
    {
        var response = await _http.GetFromJsonAsync<List<OrderDto>>($"api/customers/{customerId}/orders");
        return response ?? new List<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderStatusAsync(Guid orderId)
    {
        return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{orderId}");
    }
}

public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
