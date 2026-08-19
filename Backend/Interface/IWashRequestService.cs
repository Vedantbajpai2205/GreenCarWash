public interface IWashRequestService
{
    Task<IEnumerable<WashRequestResponseDto>> GetRequestsByWasherIdAsync(string washerId);
    Task UpdateRequestStatusAsync(int orderId, string newStatus);
    Task PopulateWashRequestsFromOrdersAsync();
}