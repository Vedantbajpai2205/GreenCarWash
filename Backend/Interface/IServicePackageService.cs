using System.Collections.Generic;
using System.Threading.Tasks;

public interface IServicePackageService
{
    Task<IEnumerable<ServicePackageResponseDto>> GetAllPackagesAsync();
    Task<ServicePackageResponseDto> GetPackageByIdAsync(int id);
    Task<ServicePackageResponseDto> CreatePackageAsync(ServicePackageCreateDto createDto);
    Task<bool> UpdatePackageAsync(ServicePackageUpdateDto updateDto);
    Task<bool> DeletePackageAsync(int id);
}