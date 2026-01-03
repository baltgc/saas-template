namespace saas_template.Common.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public object? Details { get; }

    public ApiException(string message, int statusCode = 500, object? details = null) 
        : base(message)
    {
        StatusCode = statusCode;
        Details = details;
    }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string message, object? details = null) 
        : base(message, 404, details)
    {
    }
}

public class BadRequestException : ApiException
{
    public BadRequestException(string message, object? details = null) 
        : base(message, 400, details)
    {
    }
}

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message, object? details = null) 
        : base(message, 401, details)
    {
    }
}

