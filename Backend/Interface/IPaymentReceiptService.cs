using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPaymentReceiptService
{
    Task<IEnumerable<PaymentReceiptResponseDto>> GetAllAsync();
    Task<PaymentReceiptResponseDto> GetByIdAsync(int id);
    Task<PaymentReceiptResponseDto> CreateAsync(PaymentReceiptCreateDto createDto);
    Task<bool> ValidateOrderOwnershipAsync(int orderId, string customerId);
    Task<bool> DeleteAsync(int id);
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
}