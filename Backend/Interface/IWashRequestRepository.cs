public interface IWashRequestRepository
{
    Task<IEnumerable<WashRequest>> GetAllAsync();
    Task<IEnumerable<WashRequest>> GetByWasherIdAsync(string washerId);
    Task UpdateStatusAsync(int orderId, string newStatus);
    Task PopulateWashRequestsFromOrdersAsync();
}