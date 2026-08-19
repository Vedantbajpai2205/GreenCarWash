using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
public class WasherService : IWasherService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public WasherService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<bool> AssignWasherAsync(WasherAssignmentDto dto)
    {
        var order = await _context.Orders.FindAsync(dto.OrderId);
        if (order == null) return false;

        // Optional: Validate washer existence
        var washerExists = await _context.Users.AnyAsync(u => u.Id == dto.WasherId);
        if (!washerExists) return false;

        order.WasherId = dto.WasherId;
        order.Status = "PENDING";

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<ApplicationUser?> GetWasherEmailById(string washerId)
    {
        return await _userManager.FindByIdAsync(washerId);
    }
}