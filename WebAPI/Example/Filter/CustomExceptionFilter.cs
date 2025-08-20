using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Example;

public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        Console.WriteLine(context.Exception);

        context.Result = new ObjectResult(new { message = "An unexpected error occurred." })
        {
            StatusCode = 500
        };
        
        context.ExceptionHandled = true;
    }
}