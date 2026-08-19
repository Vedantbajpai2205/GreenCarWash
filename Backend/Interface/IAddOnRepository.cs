using System.Collections.Generic;
using System.Threading.Tasks;

    public interface IAddOnRepository
    {
        Task<IEnumerable<AddOn>> GetAllAsync();
        Task<AddOn> GetByIdAsync(int id);
        Task AddAsync(AddOn addOn);
        Task UpdateAsync(AddOn addOn);
        Task DeleteAsync(AddOn addOn);
        Task<List<AddOn>> GetByIdsAsync(IEnumerable<int> ids);
    }