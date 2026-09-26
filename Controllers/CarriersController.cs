namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/carriers")]
public class CarriersController(ICarrierService carrierService) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.Public)]
    public async Task<IActionResult> GetPublicProfile(Guid userId)
    {
        var result = await carrierService.GetPublicProfileAsync(userId);
        return this.ToActionResult(result);
    }
}