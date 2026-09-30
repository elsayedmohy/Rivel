
namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("register")]
    [EnableRateLimiting(RateLimitPolicies.Sensitive)]  
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        if (result is null) return BadRequest("Email already exists");

        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        if (result is null)
            return Unauthorized("Invalid email or password");

        return Ok(result);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        var result = await authService.RefreshTokenAsync(dto.RefreshToken);
        return this.ToActionResult(result);
    }
    
    
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
    {
        var result = await authService.ConfirmEmailAsync(dto);
        return result.Succeeded ? NoContent() : this.ToActionResult(result);
    }

    [Authorize]
    [HttpPost("resend-confirmation")]
    [EnableRateLimiting(RateLimitPolicies.Sensitive)]
    public async Task<IActionResult> ResendConfirmation()
    {
        var result = await authService.ResendConfirmationAsync(User.GetUserId());
        return result.Succeeded ? NoContent() : this.ToActionResult(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting(RateLimitPolicies.Sensitive)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        await authService.ForgotPasswordAsync(dto.Email);
        return NoContent();
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting(RateLimitPolicies.Sensitive)]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var result = await authService.ResetPasswordAsync(dto);
        return result.Succeeded ? NoContent() : this.ToActionResult(result);
    }
    
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(RefreshTokenDto dto)
    {
        await authService.LogoutAsync(dto.RefreshToken);
        return NoContent();
    }
    
}