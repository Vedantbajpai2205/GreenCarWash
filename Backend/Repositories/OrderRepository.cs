using Microsoft.EntityFrameworkCore;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Car)
            .Include(o => o.Package)
            .Include(o => o.PromoCode)
            .Include(o => o.OrderAddOns)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.OrderAddOns)
            .ToListAsync();
    }
    public async Task<IEnumerable<Order>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderAddOns)
            .ToListAsync();
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderAddOns)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task AddAsync(Order order)
    {    
        try
        {
                  _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException dbex)
        {
            throw new Exception("Database update failed.", dbex);
        }
        catch(Exception ex)
        {
            throw new Exception("An unexpected error ocurred.", ex);
        }
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}