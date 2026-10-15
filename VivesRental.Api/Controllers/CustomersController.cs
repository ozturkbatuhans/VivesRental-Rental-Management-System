using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Services.Abstractions;
using VivesRental.Services.Model.Requests;
using VivesRental.Services.Model.Results;

namespace VivesRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerResult>>> GetAll()
    {
        var customers = await _customerService.Find(null);
        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResult>> GetById(Guid id)
    {
        var customer = await _customerService.Get(id);
        if (customer is null)
            return NotFound();

        return Ok(customer);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResult>> Create([FromBody] CustomerRequest request)
    {
        var created = await _customerService.Create(request);
        if (created is null)
            return BadRequest("Unable to create customer.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResult>> Update(Guid id, [FromBody] CustomerRequest request)
    {
        var updated = await _customerService.Edit(id, request);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var removed = await _customerService.Remove(id);
        if (!removed)
            return NotFound();

        return NoContent();
    }
}
