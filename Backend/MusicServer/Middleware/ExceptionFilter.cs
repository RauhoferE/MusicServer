using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Net;

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

            var exception = context.Exception;

            // ** Default 
            var statusCode = (int)HttpStatusCode.BadRequest;
            var responseMessage = exception.Message;

            // Maybe, logging the exception
            _logger.LogError(context.Exception, errorMessage);
            context.Result = new ContentResult
            {
                Content = JsonConvert.SerializeObject(
                new
                {
                        statusCode = statusCode,
                        message = responseMessage,
#if DEBUG
                        stackTrace = exception.StackTrace,
                        ExceptionType = exception.GetType().ToString().Split(".").Last(),
#endif
                        additional = @"¯\_(ツ)_/¯"
                    }
                ),
                ContentType = "application/json",
                StatusCode = statusCode
            };
        }
    }
}
