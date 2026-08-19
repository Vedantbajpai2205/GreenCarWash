public interface IOrderAddonService
{
    Task<OrderAddOn> CreateOrderAddonAsync(OrderAddonCreateDto createDto);
    Task<IEnumerable<OrderAddOn>> GetAllOrderAddonsAsync();
    Task<OrderAddOn> GetOrderAddonByIdAsync(int id);
    Task<bool> DeleteOrderAddonAsync(int id);
}