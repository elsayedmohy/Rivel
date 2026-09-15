using Microsoft.AspNetCore.Authorization;
using RiverLine.Api.Extensions;
using RiverLine.Api.Services.ShipmentRequests;

namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/shipment-requests")]
[Authorize]
public class ShipmentRequestsController(IShipmentRequestService service) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "CargoOwner")]
    public async Task<IActionResult> Create(CreateShipmentRequestDto dto,
        [FromServices] IValidator<CreateShipmentRequestDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
        
        var result = await service.CreateAsync(User.GetUserId(),dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

    }
    
    [HttpGet("open")]
    [Authorize(Roles = "Carrier")]
    public async Task<IActionResult> GetOpen() => Ok(await service.GetOpenAsync());


    [HttpGet("mine")]
    [Authorize(Roles = "CargoOwner")]
    public async Task<IActionResult> GetMine() => Ok(await service.GetMineAsync(User.GetUserId()));
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}