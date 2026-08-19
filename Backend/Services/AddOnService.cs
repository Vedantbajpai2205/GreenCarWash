using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

    public class AddOnService : IAddOnService
    {
        private readonly IAddOnRepository _repo;

        public AddOnService(IAddOnRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AddOnResponseDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(a => new AddOnResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Price = a.Price,
                IsActive = a.IsActive
            });
        }

        public async Task<AddOnResponseDto> GetByIdAsync(int id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return null;
            return new AddOnResponseDto
            {
                Id = a.Id,
                Name = a.Name,
                Price = a.Price,
                IsActive = a.IsActive
            };
        }

        public async Task<AddOnResponseDto> CreateAsync(AddOnCreateDto dto)
        {
            var a = new AddOn
            {
                Name = dto.Name,
                Price = dto.Price,
                IsActive = dto.IsActive
            };
            await _repo.AddAsync(a);
            return new AddOnResponseDto
            {
                Name = a.Name,
                Price = a.Price,
                IsActive = a.IsActive
            };
        }

        public async Task<bool> UpdateAsync(AddOnUpdateDto dto)
        {
            var a = await _repo.GetByIdAsync(dto.Id);
            if (a == null) return false;

            a.Name = dto.Name;
            a.Price = dto.Price;
            a.IsActive = dto.IsActive;

            await _repo.UpdateAsync(a);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return false;
            await _repo.DeleteAsync(a);
            return true;
        }
    }