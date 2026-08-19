using System.Collections.Generic;
using System.Threading.Tasks;

    public interface IAddOnService
    {
        Task<IEnumerable<AddOnResponseDto>> GetAllAsync();
        Task<AddOnResponseDto> GetByIdAsync(int id);
        Task<AddOnResponseDto> CreateAsync(AddOnCreateDto dto);
        Task<bool> UpdateAsync(AddOnUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }