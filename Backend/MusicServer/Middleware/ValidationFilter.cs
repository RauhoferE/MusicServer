using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MusicServer.Middleware
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before controller
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage)).ToArray();

                //var errorResponse = new ErrorResponse();

                //foreach (var error in errorsInModelState)
                //{
                //    foreach (var subError in error.Value)
                //    {
                //        var errorState = new ErrorState
                //        {
                //            FieldName = error.Key,
                //            Message = subError
                //        };
                //    }
                //}
                //TODO: implement errors
                context.Result = new BadRequestObjectResult(null);
                return;
            }

            await next();
            Console.WriteLine(context);

            // after controller
        }
    }
}
