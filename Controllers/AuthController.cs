
using Microsoft.AspNetCore.Mvc;

namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto, [FromServices] IValidator<RegisterDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(
                validationResult.ToDictionary()));

        var result = await authService.RegisterAsync(dto);
        if (result is null) return BadRequest("Email already exists");

        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto, [FromServices] IValidator<LoginDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        var result = await authService.LoginAsync(dto);
        if (result is null)
            return Unauthorized("Invalid email or password");

        return Ok(result);
    }
}