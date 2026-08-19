public interface IWasherService
{
    Task<bool> AssignWasherAsync(WasherAssignmentDto dto);
    Task<ApplicationUser?> GetWasherEmailById(string washerId);
}