using System.Collections.Generic;
using System.Threading.Tasks;
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllByUserIdAsync(string userId);
        Task<Car> GetByIdAndUserIdAsync(int id, string userId);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(Car car);
    }