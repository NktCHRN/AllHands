using System.Text.RegularExpressions;

namespace AllHands.WebApi;

public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex GetRegexPattern();

    public string? TransformOutbound(object? value)
    {
        // Slugify value
        return value is null ? null : GetRegexPattern().Replace(value.ToString()!, "$1-$2").ToLower();
    }
}
