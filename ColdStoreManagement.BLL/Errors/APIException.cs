using Microsoft.AspNetCore.Http;
using System.Net;

namespace ColdStoreManagement.BLL.Errors
{
    public class APIException 
    {
        public APIException(int statusCode, string? message = null, string? details = null)
        {
            StatusCode = statusCode;
            Message = message ?? DefaultStatusCodeMessage(statusCode);
            Details = details;
        }
        public string? Details { get; set; } // Stack trace or other details
        public int StatusCode { get; set; } 
        public string? Message { get; set; }

        private static string DefaultStatusCodeMessage(int StatusCode)
        {
            return StatusCode switch
            {
                400 => "A bad request you have made",
                401 => "Authorized you have not",
                404 => "Resource Found it was not",
                500 => "Errors are the path to the dark side.  Errors lead to anger.   Anger leads to hate.  Hate leads to career change.",
                0 => "Some Thing Went Wrong",
                _ => throw new NotImplementedException()
            };
        }

        public static APIException FromException(Exception ex, bool includeDetails)
        {
            return new APIException(
                (int)HttpStatusCode.InternalServerError,
                ex.Message,
                includeDetails ? ex.StackTrace : null // Only include stack trace in dev mode
            );
        }
    }

    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, StatusCodes.Status400BadRequest) { }
    }

    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound) { }
    }

    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, StatusCodes.Status401Unauthorized) { }
    }


}
