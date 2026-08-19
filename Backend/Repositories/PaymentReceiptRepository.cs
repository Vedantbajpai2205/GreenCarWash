using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PaymentReceiptRepository : IPaymentReceiptRepository
{
    private readonly AppDbContext _context;

    public PaymentReceiptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentReceipt>> GetAllAsync()
    {
        return await _context.PaymentReceipts.Include(r => r.Order).ToListAsync();
    }

    public async Task<PaymentReceipt> GetByIdAsync(int id)
    {
        return await _context.PaymentReceipts.Include(r => r.Order)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(PaymentReceipt receipt)
    {
        await _context.PaymentReceipts.AddAsync(receipt);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PaymentReceipt receipt)
    {
        _context.PaymentReceipts.Update(receipt);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PaymentReceipt receipt)
    {
        _context.PaymentReceipts.Remove(receipt);
        await _context.SaveChangesAsync();
    }
}