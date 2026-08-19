using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly IEmailNotificationService _emailService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService service, IEmailNotificationService emailService, ILogger<OrderController> logger)
    {
        _service = service;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "CUSTOMER,WASHER,ADMIN")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogWarning("Unauthorized access attempt to GetAll.");
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("ADMIN");
        _logger.LogInformation("Fetching all orders for user {UserId}.", userId);
        var orders = await _service.GetAllOrdersAsync(userId, isAdmin);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<ActionResult<OrderResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Fetching order with ID {OrderId}.", id);
        var order = await _service.GetOrderByIdAsync(id);
        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found.", id);
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<ActionResult<OrderResponseDto>> Create(OrderCreateDto createDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogWarning("Unauthorized access attempt to Create.");
            return Unauthorized();
        }

        var customerId = userId; // for now you can treat userId and customerId same
        _logger.LogInformation("Creating a new order for user {UserId}.", userId);
        var newOrder = await _service.CreateOrderAsync(createDto, userId, customerId);

        // Send email confirmation
        var customer = await _service.GetUserByIdAsync(userId);
        if (customer != null && !string.IsNullOrEmpty(customer.Email))
        {
            _logger.LogInformation("Sending email confirmation to {CustomerEmail}.", customer.Email);
            await _emailService.SendEmailAsync(
                customer.Email,
                "Order Confirmed",
                "Your car wash order has been confirmed and is being processed."
            );
        }
        return CreatedAtAction(nameof(GetById), new { id = newOrder.Id }, newOrder);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        _logger.LogInformation("Updating status of order {OrderId} to {Status}.", id, status);
        var updated = await _service.UpdateOrderStatusAsync(id, status);
        if (!updated)
        {
            _logger.LogWarning("Order with ID {OrderId} not found.", id);
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting order with ID {OrderId}.", id);
        var deleted = await _service.DeleteOrderAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Order with ID {OrderId} not found.", id);
            return NotFound();
        }
        return NoContent();
    }
}
