using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class PromoCodeService : IPromoCodeService
{
    private readonly AppDbContext _context;

    public PromoCodeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PromoCode>> GetAllAsync()
    {
        return await _context.PromoCodes.ToListAsync();
    }

    public async Task<PromoCode> GetByIdAsync(int id)
    {
        return await _context.PromoCodes.FindAsync(id);
    }

    public async Task<PromoCode> CreateAsync(PromoCodeCreateDto dto)
    {
        var promoCode = new PromoCode
        {
            Code = dto.Code,
            DiscountPercent = dto.DiscountPercent,
            ValidTill = dto.ValidTill,
            IsActive = true
        };

        _context.PromoCodes.Add(promoCode);
        await _context.SaveChangesAsync();
        return promoCode;
    }

    public async Task<PromoCode> UpdateAsync(int id, PromoCodeUpdateDto dto)
    {
        var promoCode = await _context.PromoCodes.FindAsync(id);
        if (promoCode == null)
            return null;

        promoCode.Code = dto.Code;
        promoCode.DiscountPercent = dto.DiscountPercent;
        promoCode.ValidTill = dto.ValidTill;
        promoCode.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return promoCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var promoCode = await _context.PromoCodes.FindAsync(id);
        if (promoCode == null)
            return false;

        _context.PromoCodes.Remove(promoCode);
        await _context.SaveChangesAsync();
        return true;
    }
}