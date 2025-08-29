namespace WebAPI.Example;

public class ExampleMiddleware
{
    private readonly RequestDelegate _next;

    public ExampleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext ctx)
    {
        Console.WriteLine("Middleware request");
        Console.WriteLine("[PATH]: " + ctx.Request.Path);
        Console.WriteLine("[STATUS]: " + ctx.Response.StatusCode);
        
        // call next middleware in pipeline
        await _next(ctx);
        
        Console.WriteLine("Middleware response");
        foreach (var header in ctx.Response.Headers)
        {
            Console.WriteLine("Response Header: {0} = {1}", header.Key, header.Value);
        }
    }
}

public static class ExampleMiddlewareExtensions
{
    public static IApplicationBuilder UseExampleMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExampleMiddleware>();
    }
}