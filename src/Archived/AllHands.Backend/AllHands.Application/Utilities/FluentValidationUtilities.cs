using FluentValidation.Results;

namespace AllHands.Application.Utilities;

public static class FluentValidationUtilities
{
    public static string FluentValidationFailuresToString(IEnumerable<ValidationFailure> failures)
    {
        return string.Join(Environment.NewLine, failures.Select(f => f.ErrorMessage));
    }
}
