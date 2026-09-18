namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/ratings")]
public class RatingsController(IRatingService service) : ControllerBase
{
    [HttpPost("create")]
    [Authorize(Roles = "CargoOwner")]
    public async Task<IActionResult> Create(
        CreateRatingDto dto)
    {
        var result = await service.CreateAsync(
            User.GetUserId(),
            dto);

        return this.ToActionResult(result);
    }

    [HttpGet("{carrierId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCarrierRatings(
        Guid carrierId)
    {
        var result = await service.GetCarrierRatingsAsync(carrierId);

        return this.ToActionResult(result);
    }
}