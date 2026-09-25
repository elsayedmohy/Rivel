namespace RiverLine.Api.Common;

public static class ControllerResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this ControllerBase controller,
        Result<T> result)
    {
        if (result.Succeeded)
            return controller.Ok(result.Data);

        return result.Error switch
        {
            OperationError.Unauthorized =>
                controller.NotFound(result.Message),
            
            OperationError.NotFound =>
                controller.NotFound(result.Message),

            OperationError.Forbidden =>
                controller.Forbid(),

            OperationError.Conflict =>
                controller.Conflict(result.Message),
            
            _ =>
                controller.BadRequest(result.Message)
        };
    }
}