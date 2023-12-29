namespace API.Errors;

public class ApiException
{
    public ApiException(int? statusCode, string? messsage, string? details)
    {
        StatusCode = statusCode;
        Message = messsage;
        Details = details;
    }

    public int? StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Details { get; set; }
}