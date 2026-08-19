using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "CUSTOMER")]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly ILogger<CarController> _logger;

    public CarController(ICarService carService, ILogger<CarController> logger)
    {
        _carService = carService;
        _logger = logger;
    }

    // GET: api/Car
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarResponseDto>>> GetAllAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to GetAllAsync.");
            return Unauthorized();
        }

        _logger.LogInformation("Fetching all cars for user {UserId}.", userId);
        var cars = await _carService.GetAllCarsAsync(userId);
        return Ok(cars);
    }

    // GET: api/Car/{id}
    [HttpGet("{id}", Name = "GetCarById")]
    public async Task<ActionResult<CarResponseDto>> GetByIdAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to GetByIdAsync for car {CarId}.", id);
            return Unauthorized();
        }

        _logger.LogInformation("Fetching car {CarId} for user {UserId}.", id, userId);
        var car = await _carService.GetCarByIdAsync(id, userId);
        if (car == null)
        {
            _logger.LogWarning("Car {CarId} not found for user {UserId}.", id, userId);
            return NotFound();
        }

        return Ok(car);
    }

    // POST: api/Car
    [HttpPost]
    public async Task<ActionResult<CarResponseDto>> CreateAsync([FromBody] CarCreateDto createDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to CreateAsync.");
            return Unauthorized();
        }

        _logger.LogInformation("Creating a new car for user {UserId}.", userId);
        var newCar = await _carService.CreateCarAsync(createDto, userId);
        return CreatedAtRoute("GetCarById", new { id = newCar.Id }, newCar);
    }

    // PUT: api/Car/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CarUpdateDto updateDto)
    {
        if (id != updateDto.Id)
        {
            _logger.LogWarning("Bad request: Car ID in URL ({UrlId}) does not match ID in body ({BodyId}).", id, updateDto.Id);
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to UpdateAsync for car {CarId}.", id);
            return Unauthorized();
        }

        _logger.LogInformation("Updating car {CarId} for user {UserId}.", id, userId);
        var updated = await _carService.UpdateCarAsync(updateDto, userId);
        if (!updated)
        {
            _logger.LogWarning("Car {CarId} not found for user {UserId}.", id, userId);
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/Car/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized access attempt to DeleteAsync for car {CarId}.", id);
            return Unauthorized();
        }

        _logger.LogInformation("Deleting car {CarId} for user {UserId}.", id, userId);
        var deleted = await _carService.DeleteCarAsync(id, userId);
        if (!deleted)
        {
            _logger.LogWarning("Car {CarId} not found for user {UserId}.", id, userId);
            return NotFound();
        }

        return NoContent();
    }
}
