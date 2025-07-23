namespace TaskManagement.Application.Models;

public class ApiResult<T>
{
    private ApiResult() { }

    private ApiResult(bool succeeded, T result, string message, IEnumerable<string> errors)
    {
        IsSuccess = succeeded;
        Data = result;
        Message = message;
        Errors = errors;
    }

    public bool IsSuccess { get; set; }

    public T Data { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public string Message { get; set; }

    public static ApiResult<T> Success(T result, string message)
    {
        return new ApiResult<T>(true, result, message, new List<string>());
    }

    public static ApiResult<T> Failure(IEnumerable<string> errors, string message)
    {
        return new ApiResult<T>(false, default, message, errors);
    }
}
