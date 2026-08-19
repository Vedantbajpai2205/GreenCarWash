using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddOnController : ControllerBase
{
    private readonly IAddOnService _service;
    private readonly ILogger<AddOnController> _logger;

    public AddOnController(IAddOnService service, ILogger<AddOnController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN/CUSTOMER")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AddOnResponseDto>>> GetAll()
    {
        _logger.LogInformation("Fetching all add-ons.");
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN/CUSTOMER")]
    [AllowAnonymous]
    public async Task<ActionResult<AddOnResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Fetching add-on with ID {Id}", id);
        var dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            _logger.LogWarning("Add-on with ID {Id} not found.", id);
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<AddOnResponseDto>> Create(AddOnCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for AddOn creation.");
            return BadRequest(ModelState);
        }

        var created = await _service.CreateAsync(dto);
        _logger.LogInformation("Add-on created with ID {Id}", created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(int id, AddOnUpdateDto dto)
    {
        if (id != dto.Id)
        {
            _logger.LogWarning("ID mismatch on update: route ID {RouteId}, DTO ID {DtoId}", id, dto.Id);
            return BadRequest("ID mismatch");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state on update for AddOn ID {Id}", id);
            return BadRequest(ModelState);
        }

        var updated = await _service.UpdateAsync(dto);
        if (!updated)
        {
            _logger.LogWarning("Update failed: Add-on with ID {Id} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Add-on with ID {Id} updated successfully.", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Delete failed: Add-on with ID {Id} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Add-on with ID {Id} deleted successfully.", id);
        return NoContent();
    }
}