public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllByUserIdAsync(string userId);
    Task<Order> GetOrderByIdAsync(int orderId);
    Task<Order> GetByIdAsync(int id);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(Order order);
    Task<IEnumerable<Order>> GetAllAsync();
}