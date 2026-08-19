public interface IOrderAddonRepository
{
    Task<OrderAddOn> AddAsync(OrderAddOn orderAddon);
    Task<IEnumerable<OrderAddOn>> GetAllAsync();
    Task<OrderAddOn> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}