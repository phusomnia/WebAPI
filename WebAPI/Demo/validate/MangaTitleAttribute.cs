using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Example.Validate;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class MangaTitleAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"{validationContext.DisplayName} is required");
    }
}