namespace WebAPI.Example;

public abstract class ABCEndpointFilter : IEndpointFilter
{
    private readonly string _methodName;

    protected ABCEndpointFilter()
    {
        _methodName = GetType().Name;
    }
    
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        Console.WriteLine($"{_methodName} before next");
        var result = next(context);
        Console.WriteLine($"{_methodName} after next");
        return result;
    }
}

class AEndpointFilter : ABCEndpointFilter
{
    public AEndpointFilter(){}
}

class BEndpointFilter : ABCEndpointFilter
{
    public BEndpointFilter(){}
}
class CEndpointFilter : ABCEndpointFilter
{
    public CEndpointFilter(){}
}