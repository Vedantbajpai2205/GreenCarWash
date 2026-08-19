using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentReceiptController : ControllerBase
{
    private readonly IPaymentReceiptService _service;
    private readonly IEmailNotificationService _emailService;
    private readonly ILogger<PaymentReceiptController> _logger;

    public PaymentReceiptController(
        IPaymentReceiptService service,
        IEmailNotificationService emailService,
        ILogger<PaymentReceiptController> logger)
    {
        _service = service;
        _emailService = emailService;
        _logger = logger;
    }

    // Admin can view all receipts
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<IEnumerable<PaymentReceiptResponseDto>>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all payment receipts.");
        var receipts = await _service.GetAllAsync();
        return Ok(receipts);
    }

    // Customer or Admin can view their own receipt by ID
    [HttpGet("{id}", Name = "GetPaymentDoneById")]
    [Authorize(Roles = "CUSTOMER,ADMIN")]
    public async Task<ActionResult<PaymentReceiptResponseDto>> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching payment receipt with ID {ReceiptId}.", id);
        var receipt = await _service.GetByIdAsync(id);
        if (receipt == null)
        {
            _logger.LogWarning("Payment receipt with ID {ReceiptId} not found.", id);
            return NotFound();
        }

        if (User.IsInRole("CUSTOMER"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (receipt.CustomerId != userId)
            {
                _logger.LogWarning("Customer {UserId} attempted to access receipt {ReceiptId} belonging to another user.", userId, id);
                return Forbid();
            }
        }
        return Ok(receipt);
    }

    // Customer can make payment (create receipt)
    [HttpPost]
    [Authorize(Roles = "CUSTOMER")]
    public async Task<ActionResult<PaymentReceiptResponseDto>> CreateAsync([FromBody] PaymentReceiptCreateDto createDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogWarning("Unauthorized access attempt to CreateAsync.");
            return Unauthorized();
        }

        _logger.LogInformation("Validating order ownership for user {UserId}.", userId);
        bool isOwner = await _service.ValidateOrderOwnershipAsync(createDto.OrderId, userId);
        if (!isOwner)
        {
            _logger.LogWarning("User {UserId} attempted to create receipt for order {OrderId} they do not own.", userId, createDto.OrderId);
            return Forbid();
        }

        _logger.LogInformation("Creating a new payment receipt for user {UserId}.", userId);
        var newReceipt = await _service.CreateAsync(createDto);
        _logger.LogInformation("Payment receipt created successfully with ID {ReceiptId}.", newReceipt.Id);

        // Send email confirmation
        var customer = await _service.GetUserByIdAsync(userId); // Assumes GetUserByIdAsync exists
        if (customer != null && !string.IsNullOrEmpty(customer.Email))
        {
            _logger.LogInformation("Sending payment receipt email to {CustomerEmail}.", customer.Email);
            await _emailService.SendEmailAsync(
                customer.Email,
                "Payment Receipt",
                $"Thank you for your payment. Your receipt ID is #{newReceipt.Id}."
            );
        }

        return CreatedAtRoute("GetPaymentDoneById", new { id = newReceipt.Id }, newReceipt);
    }

    // Admin can delete receipts
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting payment receipt with ID {ReceiptId}.", id);
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Payment receipt with ID {ReceiptId} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Payment receipt with ID {ReceiptId} deleted successfully.", id);
        return NoContent();
    }
}
