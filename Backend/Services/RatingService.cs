using Microsoft.EntityFrameworkCore;
using AutoMapper;

public class RatingService : IRatingService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RatingService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RatingResponseDto>> GetAllAsync()
    {
        var ratings = await _context.Ratings.Include(r => r.Order).ToListAsync();
        return _mapper.Map<IEnumerable<RatingResponseDto>>(ratings);
    }

    public async Task<RatingResponseDto> GetByIdAsync(int id)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating == null) return null;
        return _mapper.Map<RatingResponseDto>(rating);
    }

    public async Task<RatingResponseDto> CreateAsync(RatingCreateDto dto)
    {
        // Fetch the order first
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == dto.OrderId);
        if (order == null || !order.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Rating can only be given after order completion.");
        }

        var rating = _mapper.Map<Rating>(dto);
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return _mapper.Map<RatingResponseDto>(rating);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var rating = await _context.Ratings.FindAsync(id);
        if (rating == null) return false;

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();
        return true;
    }
}
