using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicePackageController : ControllerBase
{
    private readonly IServicePackageService _service;
    private readonly ILogger<ServicePackageController> _logger;

    public ServicePackageController(IServicePackageService service, ILogger<ServicePackageController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN/CUSTOMER")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ServicePackageResponseDto>>> GetAll()
    {
        _logger.LogInformation("Fetching all service packages.");
        var packages = await _service.GetAllPackagesAsync();
        return Ok(packages);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN/CUSTOMER")]
    public async Task<ActionResult<ServicePackageResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Fetching service package with ID {PackageId}.", id);
        var package = await _service.GetPackageByIdAsync(id);
        if (package == null)
        {
            _logger.LogWarning("Service package with ID {PackageId} not found.", id);
            return NotFound();
        }
        return Ok(package);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ServicePackageResponseDto>> Create([FromBody] ServicePackageCreateDto createDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for creating service package.");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating a new service package.");
        var createdPackage = await _service.CreatePackageAsync(createDto);
        _logger.LogInformation("Service package created successfully with ID {PackageId}.", createdPackage.Id);
        return CreatedAtAction(nameof(GetById), new { id = createdPackage.Id }, createdPackage);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(int id, [FromBody] ServicePackageUpdateDto updateDto)
    {
        if (id != updateDto.Id)
        {
            _logger.LogWarning("ID mismatch: URL ID ({UrlId}) does not match body ID ({BodyId}).", id, updateDto.Id);
            return BadRequest("ID mismatch.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for updating service package.");
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Updating service package with ID {PackageId}.", id);
        var updated = await _service.UpdatePackageAsync(updateDto);
        if (!updated)
        {
            _logger.LogWarning("Service package with ID {PackageId} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Service package with ID {PackageId} updated successfully.", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting service package with ID {PackageId}.", id);
        var deleted = await _service.DeletePackageAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Service package with ID {PackageId} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Service package with ID {PackageId} deleted successfully.", id);
        return NoContent();
    }
}
