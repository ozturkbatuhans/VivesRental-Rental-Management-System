using System.Net;
using System.Net.Http.Json;
using VivesRental.Blazor.Models;

namespace VivesRental.Blazor.Services;

public class OrderService
{
    private readonly HttpClient _http;

    public OrderService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OrderDto>> GetOrdersAsync(Guid? customerId = null, string? search = null)
    {
        var qs = new List<string>();
        if (customerId.HasValue && customerId.Value != Guid.Empty)
            qs.Add($"customerId={customerId.Value}");
        if (!string.IsNullOrWhiteSpace(search))
            qs.Add($"search={Uri.EscapeDataString(search)}");

        var url = "api/orders";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var result = await _http.GetFromJsonAsync<List<OrderDto>>(url);
        return result ?? new List<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
    }

    public async Task<OrderDto?> CreateOrderAsync(Guid customerId)
    {
        var response = await _http.PostAsync($"api/orders?customerId={customerId}", content: null);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to create order." : body);
        }

        if (response.Content.Headers.ContentLength is 0)
            return null;

        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }

    public async Task ReturnOrderAsync(Guid orderId, DateTime returnedAt)
    {
        var response = await _http.PostAsync($"api/orders/{orderId}/return?returnedAt={Uri.EscapeDataString(returnedAt.ToString("O"))}", content: null);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to return order." : body);
        }
    }
}
