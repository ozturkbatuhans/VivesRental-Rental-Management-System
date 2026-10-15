using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using VivesRental.Blazor.Models;

namespace VivesRental.Blazor.Services;

public class ProductService
{
    private readonly HttpClient _http;

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        return await GetListAsync("api/Products");
    }

    public async Task<List<ProductDto>> GetProductsAsync(DateTime availableFromUtc)
    {
        var from = availableFromUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
        var url = $"api/Products?availableFromDateTime={Uri.EscapeDataString(from)}";
        return await GetListAsync(url);
    }

    public async Task<List<ProductDto>> GetProductsAsync(DateTime availableFromUtc, DateTime availableUntilUtc)
    {
        var from = availableFromUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
        var until = availableUntilUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

        var url =
            $"api/Products?availableFromDateTime={Uri.EscapeDataString(from)}&availableUntilDateTime={Uri.EscapeDataString(until)}";

        return await GetListAsync(url);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        return await GetAsync<ProductDto>($"api/Products/{id}");
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateRequest request)
    {
        using var resp = await _http.PostAsJsonAsync("api/Products", request);
        await EnsureSuccess(resp, "Unable to create product.");
        return await ReadJsonOrThrow<ProductDto>(resp, "Invalid API response.");
    }

    public async Task<ProductDto?> UpdateProductAsync(Guid id, ProductCreateRequest request)
    {
        using var resp = await _http.PutAsJsonAsync($"api/Products/{id}", request);

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return null;

        await EnsureSuccess(resp, "Unable to update product.");

        if (resp.Content.Headers.ContentLength is 0)
            return await GetByIdAsync(id);

        return await resp.Content.ReadFromJsonAsync<ProductDto>() ?? await GetByIdAsync(id);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        using var resp = await _http.DeleteAsync($"api/Products/{id}");

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return false;

        await EnsureSuccess(resp, "Unable to delete product.");
        return true;
    }

    private async Task<List<ProductDto>> GetListAsync(string url)
    {
        var result = await GetAsync<List<ProductDto>>(url);
        return result ?? new List<ProductDto>();
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        using var resp = await _http.GetAsync(url);
        await EnsureSuccess(resp, "Request failed.");
        return await resp.Content.ReadFromJsonAsync<T>();
    }

    private static async Task EnsureSuccess(HttpResponseMessage resp, string fallbackMessage)
    {
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Unauthorized");

        if (!resp.IsSuccessStatusCode)
        {
            var content = await resp.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Request failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {content}"
            );
        }
    }

    private static async Task<T> ReadJsonOrThrow<T>(HttpResponseMessage resp, string message)
    {
        var model = await resp.Content.ReadFromJsonAsync<T>();
        if (model is null) throw new HttpRequestException(message);
        return model;
    }
}
