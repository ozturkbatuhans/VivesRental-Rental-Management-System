using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Services.Abstractions;
using VivesRental.Services.Model.Filters;
using VivesRental.Services.Model.Requests;
using VivesRental.Services.Model.Results;

namespace VivesRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products?availableFromDateTime=.&availableUntilDateTime=.
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductResult>>> GetAll(
        [FromQuery] DateTime? availableFromDateTime,
        [FromQuery] DateTime? availableUntilDateTime)
    {
        ProductFilter? filter = null;

        if (availableFromDateTime.HasValue || availableUntilDateTime.HasValue)
        {
           
            var excludeReservedAt = availableFromDateTime ?? availableUntilDateTime;

            filter = new ProductFilter
            {
                AvailableFromDateTime = availableFromDateTime,
                AvailableUntilDateTime = availableUntilDateTime,
                ExcludeReservedFromDateTime = excludeReservedAt
            };
        }

        var products = await _productService.Find(filter);
        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResult>> GetById(Guid id)
    {
        var product = await _productService.Get(id);
        if (product is null)
            return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    [ProducesResponseType(typeof(ProductResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResult>> Create([FromBody] ProductRequest request)
    {
        var created = await _productService.Create(request);
        if (created is null)
            return BadRequest("Unable to create product.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/products/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResult>> Update(Guid id, [FromBody] ProductRequest request)
    {
        var updated = await _productService.Edit(id, request);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var removed = await _productService.Remove(id);
        if (!removed)
            return NotFound();

        return NoContent();
    }

    // POST: api/products/{id}/articles/generate?amount=5
    [HttpPost("{id:guid}/articles/generate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateArticles(Guid id, [FromQuery] int amount)
    {
        if (amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        var ok = await _productService.GenerateArticles(id, amount);
        if (!ok)
            return BadRequest("Unable to generate articles.");

        return NoContent();
    }
}
