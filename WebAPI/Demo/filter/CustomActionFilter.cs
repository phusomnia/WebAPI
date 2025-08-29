using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Example;

public class CustomActionFilter : IActionFilter
{
    private Stopwatch _stopwatch = new Stopwatch();
    // Async version
    // public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    // {
    //     _stopwatch = Stopwatch.StartNew();
    //     var resultContext = await next();
    //     _stopwatch.Stop();
    //     var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
    //     _logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} Exection time: {elapsedMilliseconds} ms");
    // }

    public void OnActionExecuting(ActionExecutingContext ctx)
    {
        Console.WriteLine("ExecutionTimeLogger filter is executing.");
        _stopwatch = Stopwatch.StartNew();
    }

    public void OnActionExecuted(ActionExecutedContext ctx)
    {
        _stopwatch.Stop();
        var elapsedMs = _stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"{ctx.ActionDescriptor.DisplayName}: {elapsedMs} ms");
    }
}
