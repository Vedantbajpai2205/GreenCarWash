using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN")]
public class WasherController : ControllerBase
{
    private readonly IWasherService _washerService;
    private readonly ILogger<WasherController> _logger;
    private readonly IEmailNotificationService _emailService;

    public WasherController(IWasherService washerService, ILogger<WasherController> logger, IEmailNotificationService emailService)
    {
        _washerService = washerService;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignWasher([FromBody] WasherAssignmentDto dto)
    {
        _logger.LogInformation("Assigning washer with ID {WasherId} to order with ID {OrderId}.", dto.WasherId, dto.OrderId);
        var success = await _washerService.AssignWasherAsync(dto);
        if (!success)
        {
            _logger.LogWarning("Failed to assign washer with ID {WasherId} to order with ID {OrderId}. Order or Washer not found.", dto.WasherId, dto.OrderId);
            return NotFound("Order or Washer not found.");
        }
        var washer = await _washerService.GetWasherEmailById(dto.WasherId);
        if(!string.IsNullOrEmpty(washer.Email))
        {
            await _emailService.SendEmailAsync(
                washer.Email,
                "New Wash Assignment",
                $"You have been assigned to Order ID: {dto.OrderId}. Please check your dashboard for details."
            );
        }

        _logger.LogInformation("Washer with ID {WasherId} successfully assigned to order with ID {OrderId}.", dto.WasherId, dto.OrderId);
        return Ok("Washer successfully assigned to the order.");
    }
}
