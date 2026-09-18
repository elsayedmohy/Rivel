using RiverLine.Api.Services.Shipments;

namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize]
public class ShipmentsController(IShipmentService service) : ControllerBase
{
    [HttpPatch("{shipmentId:guid}/status")]
    [Authorize(Roles = "Carrier")]
    public async Task<IActionResult> UpdateStatus(Guid shipmentId,
        UpdateShipmentStatusDto dto)
    {
        var result = await service.UpdateStatusAsync(shipmentId, User.GetUserId(), dto.NewStatus);
        return result.Succeeded
            ? Ok(result.Data)
            : result.Error switch
            {
                ShipmentOperationError.NotFound => NotFound(result.Message),
                ShipmentOperationError.Forbidden => Forbid(),
                ShipmentOperationError.Conflict => Conflict(result.Message),
                _ => BadRequest(result.Message)
            };
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAllAsync(User.GetUserId());
        return this.ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpGet("{id}/rating")]
    public async Task<IActionResult> GetRating(Guid id)
    {
        var result = await service.GetRatingAsync(id, User.GetUserId());
        return this.ToActionResult(result);
    }
}