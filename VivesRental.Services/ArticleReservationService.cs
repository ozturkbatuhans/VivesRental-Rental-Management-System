using Microsoft.EntityFrameworkCore;
using VivesRental.Enums;
using VivesRental.Model;
using VivesRental.Repository.Core;
using VivesRental.Services.Abstractions;
using VivesRental.Services.Extensions;
using VivesRental.Services.Mappers;
using VivesRental.Services.Model.Filters;
using VivesRental.Services.Model.Requests;
using VivesRental.Services.Model.Results;

namespace VivesRental.Services;

public class ArticleReservationService : IArticleReservationService
{
    private readonly VivesRentalDbContext _context;

    public ArticleReservationService(VivesRentalDbContext context)
    {
        _context = context;
    }

    public Task<ArticleReservationResult?> Get(Guid id)
    {
        return _context.ArticleReservations
            .Where(ar => ar.Id == id)
            .MapToResults()
            .FirstOrDefaultAsync();
    }

    public Task<List<ArticleReservationResult>> Find(ArticleReservationFilter? filter = null)
    {
        return _context.ArticleReservations
            .ApplyFilter(filter)
            .MapToResults()
            .ToListAsync();
    }

    public async Task<ArticleReservationResult?> Create(ArticleReservationRequest request)
    {
        if (request.CustomerId == Guid.Empty || request.ArticleId == Guid.Empty)
        {
            return null;
        }

        // Request: DateTime?  | Entity: DateTime
        var from = request.FromDateTime ?? DateTime.Now;

        var hasUntil = request.UntilDateTime.HasValue;
        var until = hasUntil ? request.UntilDateTime!.Value : DateTime.MinValue; // store "no end" as MinValue
        var untilOrMax = hasUntil ? request.UntilDateTime!.Value : DateTime.MaxValue;

        if (hasUntil && until < from)
        {
            return null;
        }

        // Optional safety: only Normal articles can be reserved
        var article = await _context.Articles
            .Where(a => a.Id == request.ArticleId)
            .Select(a => new { a.Id, a.Status })
            .SingleOrDefaultAsync();

        if (article == null)
        {
            return null;
        }

        if (article.Status != ArticleStatus.Normal)
        {
            return null;
        }

        // Overlap check:
        // existing.From <= newUntilOrMax AND newFrom <= existing.UntilOrMax
        var hasOverlap = await _context.ArticleReservations
            .Where(ar => ar.ArticleId == request.ArticleId)
            .AnyAsync(ar =>
                ar.FromDateTime <= untilOrMax &&
                from <= (ar.UntilDateTime == DateTime.MinValue ? DateTime.MaxValue : ar.UntilDateTime)
            );

        if (hasOverlap)
        {
            return null;
        }

        var entity = new ArticleReservation
        {
            CustomerId = request.CustomerId,
            ArticleId = request.ArticleId,
            FromDateTime = from,
            UntilDateTime = until
        };

        _context.ArticleReservations.Add(entity);
        await _context.SaveChangesAsync();

        return await Get(entity.Id);
    }

    public async Task<bool> Remove(Guid id)
    {
        var exists = await _context.ArticleReservations.AnyAsync(ar => ar.Id == id);
        if (!exists)
        {
            return false;
        }

        var entity = new ArticleReservation { Id = id };
        _context.ArticleReservations.Attach(entity);
        _context.ArticleReservations.Remove(entity);

        await _context.SaveChangesAsync();
        return true;
    }
}
