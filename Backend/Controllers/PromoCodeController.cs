using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PromoCodeController : ControllerBase
{
    private readonly IPromoCodeService _promoCodeService;
    private readonly ILogger<PromoCodeController> _logger;

    public PromoCodeController(IPromoCodeService promoCodeService, ILogger<PromoCodeController> logger)
    {
        _promoCodeService = promoCodeService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN/CUSTOMER")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all promo codes.");
        var promoCodes = await _promoCodeService.GetAllAsync();
        return Ok(promoCodes);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN/CUSTOMER")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Fetching promo code with ID {PromoCodeId}.", id);
        var promoCode = await _promoCodeService.GetByIdAsync(id);
        if (promoCode == null)
        {
            _logger.LogWarning("Promo code with ID {PromoCodeId} not found.", id);
            return NotFound();
        }
        return Ok(promoCode);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create([FromBody] PromoCodeCreateDto dto)
    {
        _logger.LogInformation("Creating a new promo code.");
        var createdPromoCode = await _promoCodeService.CreateAsync(dto);
        _logger.LogInformation("Promo code created successfully with ID {PromoCodeId}.", createdPromoCode.Id);
        return CreatedAtAction(nameof(GetById), new { id = createdPromoCode.Id }, createdPromoCode);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(int id, [FromBody] PromoCodeUpdateDto dto)
    {
        _logger.LogInformation("Updating promo code with ID {PromoCodeId}.", id);
        var updatedPromoCode = await _promoCodeService.UpdateAsync(id, dto);
        if (updatedPromoCode == null)
        {
            _logger.LogWarning("Promo code with ID {PromoCodeId} not found.", id);
            return NotFound();
        }
        _logger.LogInformation("Promo code with ID {PromoCodeId} updated successfully.", id);
        return Ok(updatedPromoCode);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting promo code with ID {PromoCodeId}.", id);
        var result = await _promoCodeService.DeleteAsync(id);
        if (!result)
        {
            _logger.LogWarning("Promo code with ID {PromoCodeId} not found.", id);
            return NotFound();
        }
        _logger.LogInformation("Promo code with ID {PromoCodeId} deleted successfully.", id);
        return NoContent();
    }
}
