using System.ComponentModel.DataAnnotations;

namespace Ropufu.Homepage;

public interface IValidatable
{
    const string? Success = null;

    string? ErrorMessage { get; }
}

public class ValidatableAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(Object? value, ValidationContext validationContext)
    {
        if (value is not IValidatable x) return new ValidationResult("Object must implement IValidatable.");

        string? errorMessage = x.ErrorMessage;
        return errorMessage == IValidatable.Success ?
            ValidationResult.Success :
            new ValidationResult(errorMessage);
    }
}
