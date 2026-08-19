using Microsoft.AspNetCore.Identity;
public class OrderService : IOrderService
{
    private readonly IServicePackageRepository _packageRepo;
    private readonly IAddOnRepository _addOnRepo;
    private readonly IPromoCodeService _promoService;
    private readonly IOrderRepository _repository;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderService(IOrderRepository repository,
                        UserManager<ApplicationUser> userManager,
                        IServicePackageRepository packageRepo,
                        IAddOnRepository addOnRepo,
                        IPromoCodeService promoService)
    {
        _repository = repository;
        _userManager = userManager;
        _packageRepo = packageRepo;
        _addOnRepo = addOnRepo;
        _promoService = promoService;
    }
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }
    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync(string userId, bool isAdmin = false)
{
    IEnumerable<Order> orders;
    if (isAdmin)
    {
        orders = await _repository.GetAllAsync();
    }
    else
    {
        orders = await _repository.GetAllByUserIdAsync(userId);
    }
    return orders.Select(o => new OrderResponseDto
    {
        Id = o.Id,
        UserId = o.UserId,
        WasherId = o.WasherId,
        CarId = o.CarId,
        PackageId = o.PackageId,
        PromoCodeId = o.PromoCodeId,
        ScheduledDate = o.ScheduledDate,
        Status = o.Status,
        Location = o.Location,
        IsCompleted = o.IsCompleted,
        TotalAmount = o.TotalAmount,
        AddOnIds = o.OrderAddOns?.Select(x => x.AddOnId).ToList()
    });
}    public async Task<OrderResponseDto> GetOrderByIdAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null) return null;

        return new OrderResponseDto
        {
            Id = order.Id,
            UserId = order.UserId,
            WasherId = order.WasherId,
            CarId = order.CarId,
            PackageId = order.PackageId,
            PromoCodeId = order.PromoCodeId,
            ScheduledDate = order.ScheduledDate,
            Status = order.Status,
            Location = order.Location,
            IsCompleted = order.IsCompleted,
            TotalAmount = order.TotalAmount,
            AddOnIds = order.OrderAddOns?.Select(x => x.AddOnId).ToList()
        };
    }
    public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto createDto, string userId, string customerId)
    {
        decimal total = 0;

        var package = await _packageRepo.GetByIdAsync(createDto.PackageId);
        if (package != null) total += package.Price;

        var addOns = await _addOnRepo.GetByIdsAsync(createDto.AddOnIds);
        total += addOns.Sum(a => a.Price);

        if (createDto.PromoCodeId != null)
        {
            var promo = await _promoService.GetByIdAsync(createDto.PromoCodeId.Value);
            if (promo != null)
            {
                var discount = (promo.DiscountPercent / 100) * total;
                total -= discount;
            }
        }

        var order = new Order
        {
            UserId = userId,
            CustomerId = customerId,
            CarId = createDto.CarId,
            PackageId = createDto.PackageId,
            PromoCodeId = createDto.PromoCodeId,
            ScheduledDate = createDto.ScheduledDate,
            Location = createDto.Location,
            Status = "Pending",
            IsCompleted = false,
            TotalAmount = total,
            OrderAddOns = createDto.AddOnIds.Select(id => new OrderAddOn { AddOnId = id }).ToList()
        };

        await _repository.AddAsync(order);

        return new OrderResponseDto
        {
            Id = order.Id,
            UserId = order.UserId,
            WasherId = order.WasherId,
            CarId = order.CarId,
            PackageId = order.PackageId,
            PromoCodeId = order.PromoCodeId,
            ScheduledDate = order.ScheduledDate,
            Status = order.Status,
            Location = order.Location,
            IsCompleted = order.IsCompleted,
            TotalAmount = order.TotalAmount,
            AddOnIds = order.OrderAddOns?.Select(x => x.AddOnId).ToList()
        };
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null) return false;

        order.Status = status;
        await _repository.UpdateAsync(order);
        return true;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null) return false;

        await _repository.DeleteAsync(order);
        return true;
    }
}