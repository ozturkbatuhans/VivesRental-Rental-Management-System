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
public class ArticleReservationsController : ControllerBase
{
    private readonly IArticleReservationService _reservationService;

    public ArticleReservationsController(IArticleReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ArticleReservationResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ArticleReservationResult>>> GetAll()
    {
        var reservations = await _reservationService.Find(null);
        return Ok(reservations);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ArticleReservationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleReservationResult>> GetById(Guid id)
    {
        var reservation = await _reservationService.Get(id);
        if (reservation is null)
            return NotFound();

        return Ok(reservation);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ArticleReservationResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ArticleReservationResult>> Create([FromBody] ArticleReservationRequest request)
    {
        var created = await _reservationService.Create(request);
        if (created is null)
            return BadRequest("Unable to create reservation.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var removed = await _reservationService.Remove(id);
        if (!removed)
            return NotFound();

        return NoContent();
    }
}
