using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ServicePackageRepository : IServicePackageRepository
{
    private readonly AppDbContext _context;

    public ServicePackageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServicePackage>> GetAllAsync()
    {
        return await _context.ServicePackages.ToListAsync();
    }

    public async Task<ServicePackage> GetByIdAsync(int id)
    {
        return await _context.ServicePackages.FindAsync(id);
    }

    public async Task AddAsync(ServicePackage package)
    {
        await _context.ServicePackages.AddAsync(package);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServicePackage package)
    {
        _context.ServicePackages.Update(package);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ServicePackage package)
    {
        _context.ServicePackages.Remove(package);
        await _context.SaveChangesAsync();
    }
}