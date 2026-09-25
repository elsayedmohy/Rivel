
namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("register")]
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
    
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Name = User.Identity?.Name,
            Claims = User.Claims.Select(x => new
            {
                x.Type,
                x.Value
            })
        });
    }
    // public void SetRefreshTokenInCookie(RefreshToken refreshToken)
    // {
    //     var cookieOptions = new CookieOptions
    //     {
    //         HttpOnly = true,
    //         Expires = refreshToken.ExpireTime,
    //     };
    //     
    //     Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    // }
}