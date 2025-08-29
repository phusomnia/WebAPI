using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Example;

public class CustomResultFilter : Attribute, IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext ctx)
    {
        ctx.HttpContext.Response.Headers.Add("X-custom", "My custom value");
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        return;
    }
}