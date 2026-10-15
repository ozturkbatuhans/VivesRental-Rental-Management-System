using Microsoft.AspNetCore.Mvc;
using VivesRental.Services.Abstractions;
using VivesRental.Services.Model.Filters;
using VivesRental.Services.Model.Results;
using Microsoft.AspNetCore.Authorization;


namespace VivesRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: api/orders?customerId={guid}
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderResult>>> GetAll([FromQuery] Guid? customerId)
    {
        OrderFilter? filter = null;

        if (customerId.HasValue)
        {
            filter = new OrderFilter
            {
                CustomerId = customerId.Value
            };
        }

        var orders = await _orderService.Find(filter);
        return Ok(orders);
    }

    // GET: api/orders/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResult>> GetById(Guid id)
    {
        var order = await _orderService.Get(id);
        if (order is null)
            return NotFound();

        return Ok(order);
    }

    // POST: api/orders?customerId={guid}
    // Creates a new order header for a customer (rental order).
    [HttpPost]
    [ProducesResponseType(typeof(OrderResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResult>> Create([FromQuery] Guid customerId)
    {
        if (customerId == Guid.Empty)
            return BadRequest("customerId is required.");

        var created = await _orderService.Create(customerId);
        if (created is null)
            return BadRequest("Unable to create order.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // POST: api/orders/{id}/return?returnedAt=2026-01-10T10:00:00
    // Returns the entire order at a given time.
    [HttpPost("{id:guid}/return")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Return(Guid id, [FromQuery] DateTime returnedAt)
    {
        if (returnedAt == default)
            return BadRequest("returnedAt is required.");

        var success = await _orderService.Return(id, returnedAt);
        if (!success)
            return BadRequest("Unable to return order.");

        return NoContent();
    }
}
