using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace WebAPI_Project.Filters
{
    public class ExceptionHandleFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Create error view model with exception details
            var errorViewModel = new 
            {
                RequestId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier,
                StatusCode = "500",
                Title = "An Error Occurred",
                Message = context.Exception.Message
            };

            // Set the result to a JSON response
            context.Result = new JsonResult(errorViewModel)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = 500;
        }
    }
}
