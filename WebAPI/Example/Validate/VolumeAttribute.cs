using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace WebAPI.Example.Validate;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class VolumeAttribute : ValidationAttribute
{
    private readonly int _minVolume;

    public VolumeAttribute(int minVolume)
    {
        _minVolume = minVolume;
    }

    public override bool IsValid(object value)
    {
        if (!int.TryParse(value.ToString(), out int volume) || volume < _minVolume)
        {
            return false;
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a positive integer";
    }
}