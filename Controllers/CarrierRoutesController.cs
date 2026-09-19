
namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/routes")]
[Authorize(Roles = "Carrier")]
public class CarrierRoutesController(ICarrierRouteService routeService) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMyRoutes()
    {
        var  routes = await routeService.GetMyRoutesAsync(UserId);
        return this.ToActionResult(routes);
    }


    [HttpPost]
    public async Task<IActionResult> AddRoute([FromBody] CreateCarrierRouteDto dto)
    {
        var result = await routeService.AddRouteAsync(UserId, dto);
        return this.ToActionResult(result);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
     var result =  await routeService.DeleteRouteAsync(UserId, id);
     return this.ToActionResult(result);
    }

    [HttpGet("suggested-requests")]
    public async Task<IActionResult> GetSuggestedRequests()
    {
        var result = await routeService.GetSuggestedRequestsAsync(UserId);
        return this.ToActionResult(result);
    }
}