using RiverLine.Api.Services.Ratings;

namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/shipments/{shipmentId:guid}/rating")]
[Authorize(Roles = "CargoOwner")]
public class RatingsController(IRatingService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Guid shipmentId, CreateRatingDto dto)
    {
        var result = await service.CreateAsync(shipmentId, User.GetUserId(), dto);
        return this.ToActionResult(result);
    }
}