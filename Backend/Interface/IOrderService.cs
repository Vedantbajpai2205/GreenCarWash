public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string userId, bool isAdmin = false);
    Task<OrderResponseDto> GetOrderByIdAsync(int id);
    Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto createDto, string userId, string customerId);
    Task<bool> UpdateOrderStatusAsync(int id, string status);
    Task<bool> DeleteOrderAsync(int id);
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
}
