using Microsoft.CodeAnalysis;

namespace Stravaig.FeatureFlags.SourceGenerator;

public class StringResult
{
    public string? Value { get; }
    public string? Error { get; }
    
    public string? ErrorWithPosition => HasError 
        ? $"{SyntaxElement.SyntaxTree.GetLineSpan(SyntaxElement.Span)} caused defective source generation. {Error}"
        : null;
    public SyntaxNode SyntaxElement { get; }
    public string Position => SyntaxElement.SyntaxTree.GetLineSpan(SyntaxElement.Span).ToString();

    public bool HasError => Error.HasContent();
    public bool HasValue => Value.HasContent();

    public StringResult(SyntaxNode syntaxElement, string? value = null, string? error = null)
    {
        SyntaxElement = syntaxElement;
        Value = value;
        Error = error;
    }
}