using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ServicePackageService : IServicePackageService
{
    private readonly IServicePackageRepository _repository;

    public ServicePackageService(IServicePackageRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ServicePackageResponseDto>> GetAllPackagesAsync()
    {
        var packages = await _repository.GetAllAsync();
        return packages.Select(p => new ServicePackageResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            IsActive = p.IsActive
        });
    }

    public async Task<ServicePackageResponseDto> GetPackageByIdAsync(int id)
    {
        var package = await _repository.GetByIdAsync(id);
        if (package == null) return null;

        return new ServicePackageResponseDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Price = package.Price,
            IsActive = package.IsActive
        };
    }

    public async Task<ServicePackageResponseDto> CreatePackageAsync(ServicePackageCreateDto createDto)
    {
        var package = new ServicePackage
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            IsActive = createDto.IsActive
        };

        await _repository.AddAsync(package);

        return new ServicePackageResponseDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Price = package.Price,
            IsActive = package.IsActive
        };
    }

    public async Task<bool> UpdatePackageAsync(ServicePackageUpdateDto updateDto)
    {
        var existing = await _repository.GetByIdAsync(updateDto.Id);
        if (existing == null) return false;

        existing.Name = updateDto.Name;
        existing.Description = updateDto.Description;
        existing.Price = updateDto.Price;
        existing.IsActive = updateDto.IsActive;

        await _repository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeletePackageAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        await _repository.DeleteAsync(existing);
        return true;
    }
}