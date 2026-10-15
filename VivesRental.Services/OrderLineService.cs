using Microsoft.EntityFrameworkCore;
using VivesRental.Repository.Core;
using VivesRental.Services.Abstractions;
using VivesRental.Services.Extensions;
using VivesRental.Services.Mappers;
using VivesRental.Services.Model.Filters;
using VivesRental.Services.Model.Results;

namespace VivesRental.Services;

public class OrderLineService : IOrderLineService
{
    private readonly VivesRentalDbContext _context;

    public OrderLineService(VivesRentalDbContext context)
    {
        _context = context;
    }

    public Task<OrderLineResult?> Get(Guid id)
    {
        return _context.OrderLines
            .Where(ol => ol.Id == id)
            .MapToResults()
            .FirstOrDefaultAsync();
    }

    public Task<List<OrderLineResult>> Find(OrderLineFilter? filter = null)
    {
        return _context.OrderLines
            .ApplyFilter(filter)
            .MapToResults()
            .ToListAsync();
    }

    public async Task<bool> Rent(Guid orderId, Guid articleId)
    {
        var now = DateTime.Now;

        var articleFilter = new ArticleFilter
        {
            AvailableFromDateTime = now,
            ExcludeReservedFromDateTime = now
        };

        var article = await _context.Articles
            .Include(a => a.Product)
            .Where(a => a.Id == articleId)
            .ApplyFilter(articleFilter)
            .SingleOrDefaultAsync();

        if (article == null)
        {
            return false;
        }

        var orderLine = article.CreateOrderLine(orderId);

        _context.OrderLines.Add(orderLine);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Rent(Guid orderId, IList<Guid> articleIds)
    {
        var now = DateTime.Now;

        var articleFilter = new ArticleFilter
        {
            ArticleIds = articleIds,
            AvailableFromDateTime = now,
            ExcludeReservedFromDateTime = now
        };

        var articles = await _context.Articles
            .Include(a => a.Product)
            .ApplyFilter(articleFilter)
            .ToListAsync();

        if (articleIds.Count != articles.Count)
        {
            return false;
        }

        foreach (var article in articles)
        {
            var orderLine = article.CreateOrderLine(orderId);
            _context.OrderLines.Add(orderLine);
        }

        var numberOfObjectsUpdated = await _context.SaveChangesAsync();
        return numberOfObjectsUpdated > 0;
    }

    public async Task<bool> Return(Guid orderLineId, DateTime returnedAt)
    {
        var orderLine = await _context.OrderLines
            .Where(ol => ol.Id == orderLineId)
            .FirstOrDefaultAsync();

        if (orderLine == null)
        {
            return false;
        }

        if (returnedAt == DateTime.MinValue)
        {
            return false;
        }

        if (orderLine.ReturnedAt.HasValue)
        {
            return false;
        }

        orderLine.ReturnedAt = returnedAt;

        await _context.SaveChangesAsync();
        return true;
    }
}
