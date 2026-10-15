using System.Net;
using System.Net.Http.Json;
using VivesRental.Blazor.Models;

namespace VivesRental.Blazor.Services;

public class OrderLineService
{
    private readonly HttpClient _http;

    public OrderLineService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OrderLineDto>> GetOrderLinesByOrderIdAsync(Guid orderId)
    {
        var result = await _http.GetFromJsonAsync<List<OrderLineDto>>($"api/orderlines?orderId={orderId}");
        return result ?? new List<OrderLineDto>();
    }

    public async Task RentByProductAsync(Guid orderId, Guid productId)
    {
        // backend selects the first available article for that product
        var response = await _http.PostAsync($"api/orderlines/rent/by-product?orderId={orderId}&productId={productId}", content: null);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to rent item." : body);
        }
    }

    public async Task ReturnOrderLineAsync(Guid orderLineId, DateTime returnedAt)
    {
        var response = await _http.PostAsync($"api/orderlines/{orderLineId}/return?returnedAt={Uri.EscapeDataString(returnedAt.ToString("O"))}", content: null);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to return order line." : body);
        }
    }
}
