using Microsoft.AspNetCore.RateLimiting;

namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMine()
    {
        var result = await profileService.GetMineAsync(User.GetUserId());
        return this.ToActionResult(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMine(
        UpdateProfileDto dto)
    {
        var result = await profileService.UpdateMineAsync(User.GetUserId(), dto);
        return this.ToActionResult(result);
    }

    [HttpPost("change-password")]
    [EnableRateLimiting(RateLimitPolicies.Sensitive)]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordDto dto)
    {
        var result = await profileService.ChangePasswordAsync(User.GetUserId(), dto);
        return result.Succeeded ? NoContent() : this.ToActionResult(result);
    }
}
