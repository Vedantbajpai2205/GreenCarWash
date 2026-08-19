using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderAddonRepository : IOrderAddonRepository
{
    private readonly AppDbContext _context;

    public OrderAddonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderAddOn> AddAsync(OrderAddOn orderAddon)
    {
        _context.OrderAddOns.Add(orderAddon);
        await _context.SaveChangesAsync();
        return orderAddon;
    }

    public async Task<IEnumerable<OrderAddOn>> GetAllAsync()
    {
        return await _context.OrderAddOns
            .Include(oa => oa.Order)
            .Include(oa => oa.AddOn)
            .ToListAsync();
    }

    public async Task<OrderAddOn?> GetByIdAsync(int id)
    {
        return await _context.OrderAddOns
            .Include(oa => oa.Order)
            .Include(oa => oa.AddOn)
            .FirstOrDefaultAsync(oa => oa.Id == id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orderAddon = await _context.OrderAddOns.FindAsync(id);
        if (orderAddon == null)
            return false;

        _context.OrderAddOns.Remove(orderAddon);
        await _context.SaveChangesAsync();
        return true;
    }
}