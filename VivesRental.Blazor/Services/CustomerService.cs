using System.Net;
using System.Net.Http.Json;
using VivesRental.Blazor.Models;

namespace VivesRental.Blazor.Services;

public class CustomerService
{
    private readonly HttpClient _http;

    public CustomerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        var result = await _http.GetFromJsonAsync<List<CustomerDto>>("api/customers");
        return result ?? new List<CustomerDto>();
    }

    public async Task<CustomerDto?> GetCustomerAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/customers", request);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to create customer." : body);
        }

        return (await response.Content.ReadFromJsonAsync<CustomerDto>())
               ?? throw new InvalidOperationException("Invalid API response.");
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(Guid id, CustomerRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/customers/{id}", request);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to update customer." : body);
        }

        // Handle 204 No Content (or empty body)
        if (response.Content.Headers.ContentLength is 0)
            return await GetCustomerAsync(id);

        var updated = await response.Content.ReadFromJsonAsync<CustomerDto>();
        return updated ?? await GetCustomerAsync(id);
    }

    public async Task<bool> DeleteCustomerAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/customers/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to delete customer." : body);
        }

        return true;
    }
}
