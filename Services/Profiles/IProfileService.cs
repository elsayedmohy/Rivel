namespace RiverLine.Api.Services.Profiles;

public interface IProfileService
{
    Task<Result<ProfileDto>> GetMineAsync(Guid userId);
    Task<Result<ProfileDto>> UpdateMineAsync(Guid userId, UpdateProfileDto dto);
    Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
}
