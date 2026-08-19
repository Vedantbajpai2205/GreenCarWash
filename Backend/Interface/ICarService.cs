using System.Collections.Generic;
using System.Threading.Tasks;
    public interface ICarService
    {
        Task<IEnumerable<CarResponseDto>> GetAllCarsAsync(string userId);
        Task<CarResponseDto> GetCarByIdAsync(int id, string userId);
        Task<CarResponseDto> CreateCarAsync(CarCreateDto createDto, string userId);
        Task<bool> UpdateCarAsync(CarUpdateDto updateDto, string userId);
        Task<bool> DeleteCarAsync(int id, string userId);
    }
