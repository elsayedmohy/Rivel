namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class OffersController(IOfferService service) : ControllerBase
{
    [HttpPost("shipment-requests/{shipmentRequestId:guid}/offers")]
    [Authorize(Roles = "Carrier")]
    public async Task<IActionResult> Create(Guid shipmentRequestId, CreateOfferDto dto,
        [FromServices] IValidator<CreateOfferDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));

        var result = await service.CreateAsync(User.GetUserId(), shipmentRequestId, dto);
        return FromResult(result, data => StatusCode(StatusCodes.Status201Created, data));
    }

    [HttpGet("shipment-requests/{shipmentRequestId:guid}/offers")]
    [Authorize(Roles = "CargoOwner")]
    public async Task<IActionResult> GetOffersForRequest(Guid shipmentRequestId)
    {
        var result = await service.GetForRequestAsync(shipmentRequestId, User.GetUserId());
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("offers/mine")]
    [Authorize(Roles = "Carrier")]
    public async Task<IActionResult> GetMine() => Ok(await service.GetMineAsync(User.GetUserId()));

    [HttpPost("offers/{offerId:guid}/accept")]
    [Authorize(Roles = "CargoOwner")]
    public async Task<IActionResult> Accept(Guid offerId)
    {
        var result = await service.AcceptAsync(User.GetUserId(), offerId);
        return FromResult(result, Ok);
    }

    private IActionResult FromResult(OfferOperationResult result, Func<OfferDto, IActionResult> onSuccess) =>
        result.Succeeded
            ? onSuccess(result.Data!)
            : result.Error switch
            {
                OfferOperationError.NotFound => NotFound(result.Message),
                OfferOperationError.Conflict => Conflict(result.Message),
                _ => BadRequest(result.Message)
            };
}