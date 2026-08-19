using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
    public class CarRepository : ICarRepository
    {
        private readonly AppDbContext _context;

        public CarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Car>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Cars
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
        
        public async Task<Car> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }
        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Car car)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }