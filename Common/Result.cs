namespace RiverLine.Api.Common;

public class Result<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public OperationError Error { get; init; }
    public string? Message { get; init; }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static Result<T> Failure(OperationError error, string message) =>
        new() { Succeeded = false, Error = error, Message = message };

    public static implicit operator Result<T>(Result.FailureResult failure) =>
        new() { Succeeded = false, Error = failure.Error, Message = failure.Message };
}

public static class Result
{
    public readonly struct FailureResult
    {
        public OperationError Error { get; init; }
        public string Message { get; init; }
    }

    public static FailureResult Failure(OperationError error, string message) =>
        new() { Error = error, Message = message };
}