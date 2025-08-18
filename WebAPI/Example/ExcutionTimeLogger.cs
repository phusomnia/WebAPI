using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Example;

public class ExecutionTimeLogger : Attribute, IActionFilter
{
    private Stopwatch _stopwatch;
    private readonly ILogger<ExecutionTimeLogger> _logger;

    public ExecutionTimeLogger(ILogger<ExecutionTimeLogger> logger)
    {
        _logger = logger;
    }
    
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
        _logger.LogInformation("ExecutionTimeLogger filter is executing.");
        _stopwatch = Stopwatch.StartNew();
    }
    
    public void OnActionExecuted(ActionExecutedContext ctx)
    {
        _stopwatch.Stop();
        var elapsedMs = _stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"{ctx.ActionDescriptor.DisplayName}: {elapsedMs} ms");
        _logger.LogInformation($"{ctx.ActionDescriptor.DisplayName}: {elapsedMs} ms");
    }
}