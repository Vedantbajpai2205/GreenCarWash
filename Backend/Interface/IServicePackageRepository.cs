using System.Collections.Generic;
using System.Threading.Tasks;

public interface IServicePackageRepository
{
    Task<IEnumerable<ServicePackage>> GetAllAsync();
    Task<ServicePackage> GetByIdAsync(int id);
    Task AddAsync(ServicePackage package);
    Task UpdateAsync(ServicePackage package);
    Task DeleteAsync(ServicePackage package);
}