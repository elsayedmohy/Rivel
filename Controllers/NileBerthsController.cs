
namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/berths")]

public class NileBerthsController(INileBerthService berthService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await berthService.GetNileBerthsAsync();
        return this.ToActionResult(result);
    }
}