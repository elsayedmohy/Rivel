
namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize]
public class ShipmentsController(IShipmentService service,
    IRatingService ratingService) : ControllerBase
{
    [HttpPatch("{shipmentId:guid}/status")]
    [Authorize(Roles = "Carrier")]
    public async Task<IActionResult> UpdateStatus(Guid shipmentId,
        UpdateShipmentStatusDto dto)
    {
        var result = await service.UpdateStatusAsync(shipmentId, User.GetUserId(), dto.NewStatus);
        return this.ToActionResult(result);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? rated = null)
    {
        var result = await service.GetAllAsync(User.GetUserId(),  rated);
        return this.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await service.GetByIdAsync(id, User.GetUserId());
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpGet("{id:guid}/rating")]
    public async Task<IActionResult> GetRating(Guid id)
    {
        var result = await ratingService.GetRatingAsync(id, User.GetUserId());
        return this.ToActionResult(result);
    }
    
    [HttpGet("{id:guid}/contact")]
    public async Task<IActionResult> GetContact(Guid id)
    {
        Response.Headers.CacheControl = "no-store";   // personal data: no browser/proxy caching
        var result = await service.GetContactAsync(id, User.GetUserId());
        return this.ToActionResult(result);
    }
}