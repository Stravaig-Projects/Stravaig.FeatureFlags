namespace Stravaig.FeatureFlags.SourceGenerator;

internal static class StringExtensions
{
    internal static bool HasContent(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}