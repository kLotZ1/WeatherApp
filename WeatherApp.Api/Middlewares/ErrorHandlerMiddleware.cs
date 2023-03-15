namespace WeatherApp.Api.Middlewares
{
    public class ErrorHandler
    {
        private readonly RequestDelegate _requestDelegate;
        private ILogger _logger;

        public ErrorHandler(RequestDelegate requestDelegate, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ILogger>();
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                _logger.Log(LogLevel.Error, "Unexpected error.", ex);

                await context.Response.WriteAsJsonAsync(ex.Message);
            }
        }
    }
}