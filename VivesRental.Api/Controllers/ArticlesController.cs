using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VivesRental.Enums;
using VivesRental.Services.Abstractions;
using VivesRental.Services.Model.Filters;
using VivesRental.Services.Model.Requests;
using VivesRental.Services.Model.Results;

namespace VivesRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    // GET: api/articles?productId={guid}
    [HttpGet]
    [ProducesResponseType(typeof(List<ArticleResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ArticleResult>>> GetAll([FromQuery] Guid? productId)
    {
        ArticleFilter? filter = null;

        if (productId.HasValue)
        {
            filter = new ArticleFilter
            {
                ProductId = productId.Value
            };
        }

        var articles = await _articleService.Find(filter);
        return Ok(articles);
    }

    // GET: api/articles/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ArticleResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleResult>> GetById(Guid id)
    {
        var article = await _articleService.Get(id);
        if (article is null)
            return NotFound();

        return Ok(article);
    }

    // POST: api/articles
    [HttpPost]
    [ProducesResponseType(typeof(ArticleResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ArticleResult>> Create([FromBody] ArticleRequest request)
    {
        var created = await _articleService.Create(request);
        if (created is null)
            return BadRequest("Unable to create article.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/articles/{id}/status?status=Available
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] ArticleStatus status)
    {
        var success = await _articleService.UpdateStatus(id, status);
        if (!success)
            return BadRequest("Unable to update article status.");

        return NoContent();
    }


    // DELETE: api/articles/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var removed = await _articleService.Remove(id);
        if (!removed)
            return NotFound();

        return NoContent();
    }
}
