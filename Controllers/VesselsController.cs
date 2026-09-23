namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/vessels")]
[Authorize(Roles = nameof(UserRole.Carrier))]
public class VesselsController(IVesselService vesselService) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var result = await vesselService.GetMineAsync(User.GetUserId());
            return this.ToActionResult(result);
    }
 
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVesselDto request)
    {
        var result =await vesselService.CreateAsync(User.GetUserId(), request);
        return this.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVesselDto request)
    {
        var result = await vesselService.UpdateAsync(User.GetUserId(), id, request);
        return this.ToActionResult(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetVesselStatusRequest request)
    {
        var result = await vesselService.SetStatusAsync(User.GetUserId(), id, request.Status);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Archive(Guid id)
    {
        var result = await vesselService.ArchiveAsync(User.GetUserId(), id);
        return this.ToActionResult(result);
    }
}