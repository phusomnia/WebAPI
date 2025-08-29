using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Core.handlers;

public static class CustomInvalidModelState
{
    public static IActionResult BuildResponse(ActionContext context)
    {
        var errors = context.ModelState
            .Where(ms => ms.Value.Errors.Count > 0 && ms.Key != "dto")
            .ToDictionary(
                kvp => kvp.Key.Split(".").Last(),
                kvp => kvp.Value.Errors.Select(err => 
                    err.ErrorMessage.Contains("could not be converted to ")
                    ? $"{kvp.Key.Split(".").Last()} must be {formatTypeError(err.ErrorMessage, kvp.Key)}"
                    : err.ErrorMessage
                ).FirstOrDefault()
            );

        var problemDetails = new  
        {
            Title = "Failed",
            Status = StatusCodes.Status400BadRequest,
            Errors = errors
        };

        return new BadRequestObjectResult(problemDetails);
    }

    private static String formatTypeError(string rawError, string fieldName)
    {
        String marker = "System.";
        Int32 idx = rawError.IndexOf("System.", StringComparison.Ordinal);
        if (idx > 0)
        {
            String type = rawError.Substring(idx + marker.Length).Split(".")[0];
            return convertTypeFormat(type);
        }

        return $"{fieldName} has an invalid format";
    }

    private static String convertTypeFormat(String typeName) 
    {
        return typeName switch
        {
            "Int32" => "int",
            "String" => "string",
            "Double" => "number",
            _ => typeName.ToLowerInvariant()
        };
    }
}