using Microsoft.AspNetCore.Mvc.Filters;

namespace MusicServer.Middleware
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            //TODO: Handle Errors
            // Some logic to handle specific exceptions
            var errorMessage = context.Exception.Message;

            // Maybe, logging the exception
            _logger.LogError(context.Exception, errorMessage);
        }
    }
}
