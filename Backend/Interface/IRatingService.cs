public interface IRatingService
{
    Task<IEnumerable<RatingResponseDto>> GetAllAsync();
    Task<RatingResponseDto> GetByIdAsync(int id);
    Task<RatingResponseDto> CreateAsync(RatingCreateDto dto);
    Task<bool> DeleteAsync(int id);
}