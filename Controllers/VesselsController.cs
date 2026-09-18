using RiverLine.Api.Services.Vessels;

namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/vessels")]
[Authorize(Roles = "Carrier")]
public class VesselsController(IVesselService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateVesselDto dto,
        [FromServices] IValidator<CreateVesselDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(
                new ValidationProblemDetails(validation.ToDictionary()));

        var result = await service.CreateAsync(User.GetUserId(), dto);
        return this.ToActionResult(result);
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetMineAsync(User.GetUserId());
        return this.ToActionResult(result);
    }
}