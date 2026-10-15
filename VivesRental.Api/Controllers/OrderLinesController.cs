using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Services.Abstractions;
using VivesRental.Services.Model.Filters;

namespace VivesRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderLinesController : ControllerBase
{
    private readonly IOrderLineService _orderLineService;
    private readonly IArticleService _articleService;
    private readonly IProductService _productService;

    public OrderLinesController(
        IOrderLineService orderLineService,
        IArticleService articleService,
        IProductService productService)
    {
        _orderLineService = orderLineService;
        _articleService = articleService;
        _productService = productService;
    }

    // GET: api/orderlines
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderLineFilter? filter)
    {
        var lines = await _orderLineService.Find(filter);
        return Ok(lines);
    }

    // GET: api/orderlines/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var line = await _orderLineService.Get(id);
        if (line is null)
            return NotFound();

        return Ok(line);
    }

    // POST: api/orderlines/rent/by-product?orderId={guid}&productId={guid}
    [HttpPost("rent/by-product")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RentByProduct([FromQuery] Guid orderId, [FromQuery] Guid productId)
    {
        if (orderId == Guid.Empty || productId == Guid.Empty)
            return BadRequest("orderId and productId are required.");

        var product = await _productService.Get(productId);
        if (product is null)
            return NotFound("Product not found.");

        var rentedAt = DateTime.UtcNow;
        var expiresAt = rentedAt.AddDays(product.RentalExpiresAfterDays);

        var availableArticles = await _articleService.Find(new ArticleFilter
        {
            ProductId = productId,
            AvailableFromDateTime = rentedAt,
            AvailableUntilDateTime = expiresAt,

            ExcludeReservedFromDateTime = rentedAt
        });


        var firstFreeArticle = availableArticles.FirstOrDefault();
        if (firstFreeArticle is null)
            return BadRequest("No available articles for this product.");

        var success = await _orderLineService.Rent(orderId, firstFreeArticle.Id);
        if (!success)
            return BadRequest("Unable to rent article.");

        return NoContent();
    }

    // POST: api/orderlines/rent?orderId={guid}&articleId={guid}
    [HttpPost("rent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RentSingle([FromQuery] Guid orderId, [FromQuery] Guid articleId)
    {
        if (orderId == Guid.Empty || articleId == Guid.Empty)
            return BadRequest("orderId and articleId are required.");

        var success = await _orderLineService.Rent(orderId, articleId);
        if (!success)
            return BadRequest("Unable to rent article.");

        return NoContent();
    }

    // POST: api/orderlines/rent/bulk?orderId={guid}
    // Body: ["guid1","guid2"]
    [HttpPost("rent/bulk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RentBulk([FromQuery] Guid orderId, [FromBody] List<Guid> articleIds)
    {
        if (orderId == Guid.Empty)
            return BadRequest("orderId is required.");

        if (articleIds is null || articleIds.Count == 0)
            return BadRequest("articleIds are required.");

        var success = await _orderLineService.Rent(orderId, articleIds);
        if (!success)
            return BadRequest("Unable to rent articles.");

        return NoContent();
    }

    // POST: api/orderlines/{id}/return?returnedAt=2025-01-01T10:00:00Z
    [HttpPost("{id:guid}/return")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Return(Guid id, [FromQuery] DateTime returnedAt)
    {
        if (returnedAt == default)
            return BadRequest("returnedAt is required.");

        var success = await _orderLineService.Return(id, returnedAt);
        if (!success)
            return BadRequest("Unable to return order line.");

        return NoContent();
    }
}
