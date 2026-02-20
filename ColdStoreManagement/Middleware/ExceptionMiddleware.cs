using ColdStoreManagement.BLL.Errors;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ColdStoreManagement.Middleware
{
    /// <summary>
    /// Custom Exception Middleware
    /// </summary>
    /// <param name="next"></param>
    /// <param name="hostEnvironment"></param>
    public class ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment hostEnvironment)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

        /// <summary>
        /// Middleware invoke function
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong {FunctionName}: {ex.Message}");
                // _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            string message;

            switch (ex)
            {
                case AppException appEx:
                    statusCode = appEx.StatusCode;
                    message = appEx.Message;
                    break;

                case SqlException:
                    statusCode = StatusCodes.Status503ServiceUnavailable;
                    message = "Database service is unavailable";
                    break;

                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "Unauthorized access";
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "An unexpected error occurred";
                    break;
            }

            _logger.LogError(
                ex,
                "Unhandled exception | TraceId: {TraceId} | Path: {Path}",
                context.TraceIdentifier,
                context.Request.Path
            );

            var response = new APIErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                TraceId = context.TraceIdentifier,
                Details = ex.ToString()
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            );
        }


        /// <summary>
        /// this function used to get running method name
        /// </summary>
        //private static string FunctionName
        //{
        //    get
        //    {
        //        try
        //        {
        //            var st = new System.Diagnostics.StackTrace();
        //            var sf = st.GetFrame(1);

        //            // Check for null BEFORE accessing sf.GetMethod()
        //            if (sf != null)
        //            {
        //                var currentMethodName = sf.GetMethod();
        //                if (currentMethodName != null) // Also check if currentMethodName is null (unlikely but possible)
        //                {
        //                    if (currentMethodName.Name == "MoveNext")
        //                        return currentMethodName.ReflectedType?.FullName ?? string.Empty; // Null-conditional operator and null-coalescing
        //                    else
        //                        return currentMethodName.Name;
        //                }
        //            }

        //            return string.Empty; // Return empty if sf or currentMethodName is null

        //        }
        //        catch (Exception)
        //        {
        //            return string.Empty; // Return empty on error
        //        }
        //    }
        //}
    }
}
