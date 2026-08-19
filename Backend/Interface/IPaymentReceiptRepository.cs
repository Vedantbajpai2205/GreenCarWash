using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPaymentReceiptRepository
{
    Task<IEnumerable<PaymentReceipt>> GetAllAsync();
    Task<PaymentReceipt> GetByIdAsync(int id);
    Task AddAsync(PaymentReceipt receipt);
    Task UpdateAsync(PaymentReceipt receipt);
    Task DeleteAsync(PaymentReceipt receipt);
}