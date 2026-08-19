using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PaymentReceiptService : IPaymentReceiptService
{
    private readonly IPaymentReceiptRepository _repository;
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public PaymentReceiptService(IPaymentReceiptRepository repository, AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }
    public async Task<IEnumerable<PaymentReceiptResponseDto>> GetAllAsync()
    {
        var receipts = await _repository.GetAllAsync();
        return receipts.Select(r => new PaymentReceiptResponseDto
        {
            Id = r.Id,
            OrderId = r.OrderId,
            PaymentDate = r.PaymentDate,
            AmountPaid = r.AmountPaid,
            PaymentStatus = r.PaymentStatus,
            CustomerId = r.Order?.CustomerId
        });
    }

    public async Task<PaymentReceiptResponseDto> GetByIdAsync(int id)
    {
        var receipt = await _context.PaymentReceipts
            .Include(r => r.Order)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receipt == null) return null;

        return new PaymentReceiptResponseDto
        {
            Id = receipt.Id,
            OrderId = receipt.OrderId,
            PaymentDate = receipt.PaymentDate,
            AmountPaid = receipt.AmountPaid,
            PaymentStatus = receipt.PaymentStatus,
            CustomerId = receipt.Order?.CustomerId
        };
    }

    public async Task<PaymentReceiptResponseDto> CreateAsync(PaymentReceiptCreateDto createDto)
    {
        var receipt = new PaymentReceipt
        {
            OrderId = createDto.OrderId,
            AmountPaid = createDto.AmountPaid,
            PaymentDate = DateTime.UtcNow,
            PaymentStatus = createDto.PaymentStatus
        };

        await _repository.AddAsync(receipt);

        // Fetch order to get CustomerId for response
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == receipt.OrderId);

        return new PaymentReceiptResponseDto
        {
            Id = receipt.Id,
            OrderId = receipt.OrderId,
            PaymentDate = receipt.PaymentDate,
            AmountPaid = receipt.AmountPaid,
            PaymentStatus = receipt.PaymentStatus,
            CustomerId = order?.CustomerId
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var receipt = await _repository.GetByIdAsync(id);
        if (receipt == null) return false;

        await _repository.DeleteAsync(receipt);
        return true;
    }

    public async Task<bool> ValidateOrderOwnershipAsync(int orderId, string customerId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        return order != null && order.CustomerId == customerId;
    }
}