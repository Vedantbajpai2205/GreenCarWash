using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

    public class AddOnRepository : IAddOnRepository
    {
        private readonly AppDbContext _context;

        public AddOnRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddOn>> GetAllAsync()
        {
            return await _context.AddOns.ToListAsync();
        }

        public async Task<AddOn> GetByIdAsync(int id)
        {
            return await _context.AddOns.FindAsync(id);
        }

        public async Task AddAsync(AddOn addOn)
        {
            await _context.AddOns.AddAsync(addOn);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AddOn addOn)
        {
            _context.AddOns.Update(addOn);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AddOn addOn)
        {
            _context.AddOns.Remove(addOn);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AddOn>> GetByIdsAsync(IEnumerable<int>ids){
            return await _context.AddOns.Where(addOn=>ids.Contains(addOn.Id)).ToListAsync();
        }
    }