using Microsoft.EntityFrameworkCore;
public class WashRequestRepository : IWashRequestRepository
{
    private readonly AppDbContext _context;
    private readonly IEmailNotificationService _emailNotificationService;

    public WashRequestRepository(AppDbContext context, IEmailNotificationService emailNotificationService)
    {
        _context = context;
        _emailNotificationService = emailNotificationService;
    }

    public async Task<IEnumerable<WashRequest>> GetAllAsync()
    {
    return await _context.WashRequests
        .Include(w => w.Order)
        .Include(w => w.User) // <-- Include the customer (user)
        .ToListAsync();
    }

    public async Task<IEnumerable<WashRequest>> GetByWasherIdAsync(string washerId)
    {
        return await _context.WashRequests
            .Include(w => w.Order)
            .Where(w => w.WasherId == washerId)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(int orderId, string newStatus)
    {
        var request = await _context.WashRequests.FirstOrDefaultAsync(r => r.OrderId == orderId);
        if (request != null)
        {
            request.Status = newStatus.ToUpper();
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if(order != null)
            {
                order.Status = newStatus.ToUpper();
            }
            if(newStatus.ToUpper().Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                order.IsCompleted = true;
            }
            if(newStatus.ToUpper().Equals("DECLINED", StringComparison.OrdinalIgnoreCase))
            {
                order.WasherId = null;
            }
            await _context.SaveChangesAsync();

            // Get customer's email
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (customer != null)
            {
                string subject = "Wash Request Update";
                string message = newStatus.ToUpper() switch
                {
                    "ACCEPTED" => "Your car wash request has been accepted by the washer.",
                    "DECLINED" => "Unfortunately, your car wash request has been declined by the washer.",
                    "COMPLETED" => "Your car wash has been successfully completed.",
                    _ => $"The status of your car wash request has been updated to: {newStatus}."
                };

                await _emailNotificationService.SendEmailAsync(customer.Email, subject, message);
            }
        }
    }

    public async Task PopulateWashRequestsFromOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderAddOns)
            .ToListAsync();

        foreach (var order in orders)
        {
            if (await _context.WashRequests.AnyAsync(w => w.OrderId == order.Id))
                continue;

            var washRequest = new WashRequest
            {
                OrderId = order.Id,
                UserId = order.CustomerId,
                WasherId = order.WasherId,
                CarId = order.CarId,
                PackageId = order.PackageId,
                PromoCodeId = order.PromoCodeId,
                ScheduledDate = order.ScheduledDate,
                Status = "PENDING",
                OrderAddons = order.OrderAddOns
            };

            _context.WashRequests.Add(washRequest);
        }

        await _context.SaveChangesAsync();
    }
}
