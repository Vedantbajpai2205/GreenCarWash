using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WashRequestController : ControllerBase
{
    private readonly IWashRequestService _service;
    private readonly ILogger<WashRequestController> _logger;

    public WashRequestController(IWashRequestService service, ILogger<WashRequestController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("washer")]
    [Authorize(Roles = "WASHER")]
    public async Task<IActionResult> GetByWasher()
    {
        var washerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Fetching wash requests for washer with ID {WasherId}.", washerId);
        var result = await _service.GetRequestsByWasherIdAsync(washerId);
        return Ok(result);
    }

    [HttpPut("status/{orderId}")]
    [Authorize(Roles = "WASHER")]
    public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] string newStatus)
    {
        _logger.LogInformation("Updating status of wash request with order ID {OrderId} to {NewStatus}.", orderId, newStatus);
        await _service.UpdateRequestStatusAsync(orderId, newStatus);
        _logger.LogInformation("Status of wash request with order ID {OrderId} updated to {NewStatus}.", orderId, newStatus);
        return NoContent();
    }

    [HttpPost("populate")]
    [Authorize(Roles = "WASHER")]
    public async Task<IActionResult> PopulateFromOrders()
    {
        _logger.LogInformation("Populating wash requests from orders.");
        await _service.PopulateWashRequestsFromOrdersAsync();
        _logger.LogInformation("Wash requests populated from orders successfully.");
        return Ok("Wash requests populated from orders.");
    }
}
