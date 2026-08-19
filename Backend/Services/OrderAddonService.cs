using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderAddonService : IOrderAddonService
{
    private readonly IOrderAddonRepository _repository;

    public OrderAddonService(IOrderAddonRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderAddOn> CreateOrderAddonAsync(OrderAddonCreateDto createDto)
    {
        var orderAddon = new OrderAddOn
        {
            OrderId = createDto.OrderId,
            AddOnId = createDto.AddonId
        };

        return await _repository.AddAsync(orderAddon);
    }

    public async Task<IEnumerable<OrderAddOn>> GetAllOrderAddonsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<OrderAddOn> GetOrderAddonByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> DeleteOrderAddonAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}