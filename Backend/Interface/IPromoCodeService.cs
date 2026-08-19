using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPromoCodeService
{
    Task<IEnumerable<PromoCode>> GetAllAsync();
    Task<PromoCode> GetByIdAsync(int id);
    Task<PromoCode> CreateAsync(PromoCodeCreateDto dto);
    Task<PromoCode> UpdateAsync(int id, PromoCodeUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}