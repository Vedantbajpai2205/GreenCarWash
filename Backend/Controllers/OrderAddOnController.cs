using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class OrderAddonController : ControllerBase
{
    private readonly IOrderAddonService _service;
    private readonly ILogger<OrderAddonController> _logger;

    public OrderAddonController(IOrderAddonService service, ILogger<OrderAddonController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderAddonCreateDto createDto)
    {
        if (createDto == null)
        {
            _logger.LogWarning("Create request with null OrderAddonCreateDto.");
            return BadRequest();
        }

        _logger.LogInformation("Creating a new order addon.");
        var created = await _service.CreateOrderAddonAsync(createDto);
        _logger.LogInformation("Order addon created successfully with ID {OrderAddonId}.", created.Id);
        return Ok(created);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderAddOn>>> GetAll()
    {
        _logger.LogInformation("Fetching all order addons.");
        var orderAddons = await _service.GetAllOrderAddonsAsync();
        return Ok(orderAddons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderAddOn>> GetById(int id)
    {
        _logger.LogInformation("Fetching order addon with ID {OrderAddonId}.", id);
        var orderAddon = await _service.GetOrderAddonByIdAsync(id);
        if (orderAddon == null)
        {
            _logger.LogWarning("Order addon with ID {OrderAddonId} not found.", id);
            return NotFound();
        }
        return Ok(orderAddon);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting order addon with ID {OrderAddonId}.", id);
        var deleted = await _service.DeleteOrderAddonAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Order addon with ID {OrderAddonId} not found.", id);
            return NotFound();
        }
        _logger.LogInformation("Order addon with ID {OrderAddonId} deleted successfully.", id);
        return NoContent();
    }
}
