using AutoMapper;
public class WashRequestService : IWashRequestService
{
    private readonly IWashRequestRepository _repository;
    private readonly IMapper _mapper;

    public WashRequestService(IWashRequestRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WashRequestResponseDto>> GetRequestsByWasherIdAsync(string washerId)
    {
        var data = await _repository.GetByWasherIdAsync(washerId);
        return _mapper.Map<IEnumerable<WashRequestResponseDto>>(data);
    }

    public async Task UpdateRequestStatusAsync(int orderId, string newStatus)
    {
        await _repository.UpdateStatusAsync(orderId, newStatus);
    }

    public async Task PopulateWashRequestsFromOrdersAsync()
    {
        await _repository.PopulateWashRequestsFromOrdersAsync();
    }
}
