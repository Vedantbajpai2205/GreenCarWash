using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RatingController : ControllerBase
{
    private readonly IRatingService _service;
    private readonly ILogger<RatingController> _logger;

    public RatingController(IRatingService service, ILogger<RatingController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RatingResponseDto>>> GetAll()
    {
        _logger.LogInformation("Fetching all ratings.");
        var ratings = await _service.GetAllAsync();
        return Ok(ratings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RatingResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Fetching rating with ID {RatingId}.", id);
        var rating = await _service.GetByIdAsync(id);
        if (rating == null)
        {
            _logger.LogWarning("Rating with ID {RatingId} not found.", id);
            return NotFound();
        }
        return Ok(rating);
    }

    [HttpPost]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<ActionResult<RatingResponseDto>> Create(RatingCreateDto dto)
    {
        _logger.LogInformation("Creating a new rating.");
        var created = await _service.CreateAsync(dto);
        _logger.LogInformation("Rating created successfully with ID {RatingId}.", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting rating with ID {RatingId}.", id);
        var result = await _service.DeleteAsync(id);
        if (!result)
        {
            _logger.LogWarning("Rating with ID {RatingId} not found.", id);
            return NotFound();
        }
        _logger.LogInformation("Rating with ID {RatingId} deleted successfully.", id);
        return NoContent();
    }
}
