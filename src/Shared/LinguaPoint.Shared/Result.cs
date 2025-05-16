namespace LinguaPoint.Shared;

public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true);
    public static Result Failure(string errorMessage) => new(false, errorMessage);
}

public class Result<TResult>
{
    public bool IsSuccess { get; }
    public TResult? Data { get; }
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, TResult? data = default, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }

    public static Result<TResult> Success(TResult result) => new(true, result);
    public static Result<TResult> Failure(string errorMessage) => new(false, default, errorMessage);
}