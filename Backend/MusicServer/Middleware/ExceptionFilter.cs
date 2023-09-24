using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MusicServer.Exceptions;
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
            var responseMessage = "";

            # if DEBUG
            responseMessage = exception.Message;
            #endif

            if (exception.GetType() == typeof(PlaylistNotFoundException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = "Playlist was not found.";
            }


            if (exception.GetType() == typeof(UserNotFoundException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = "User was not found.";
            }


            if (exception.GetType() == typeof(PlaylistNotFoundException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = "Playlist was not found.";
            }


            if (exception.GetType() == typeof(NotAllowedException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = "Action is not allowed.";
            }


            if (exception.GetType() == typeof(PlayListAlreadyInUseException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = "Playlist was already added to user.";
            }


            if (exception.GetType() == typeof(SongNotFoundException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = "Song was not found.";
            }


            if (exception.GetType() == typeof(AlbumNotFoundException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = "Album was not found.";
            }

            if (exception.GetType() == typeof(AuthenticationServiceException))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                responseMessage = exception.Message;
            }

            if (exception.GetType() == typeof(UnauthenticatedException))
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                responseMessage = exception.Message;
            }

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
