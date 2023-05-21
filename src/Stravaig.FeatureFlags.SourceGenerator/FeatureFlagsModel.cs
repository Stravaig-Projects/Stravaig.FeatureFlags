using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stravaig.FeatureFlags.SourceGenerator;

internal class FeatureFlagsModel : IEquatable<FeatureFlagsModel>
{
    internal string? NamespaceName { get; init; }
    internal string EnumName { get; init; }
    internal ImmutableArray<string> FeatureFlagNames { get; init; }
    internal bool IncludeTestFakes { get; init; }
    
    
    internal static FeatureFlagsModel? Create(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        var node = (EnumDeclarationSyntax)ctx.Node;
        var enumSymbol = ctx.SemanticModel.GetDeclaredSymbol(node, ct);
        if (enumSymbol == null)
            return null;
        
        var enumAttr = GetFeatureAttribute(ctx, enumSymbol);
        if (enumAttr == null)
            return null;

        var featureFlagNames = GetFeatureFlagNames(enumSymbol);
        if (featureFlagNames.Length == 0)
            return null;
        
        string namespaceName = enumSymbol.ContainingNamespace.ToDisplayString();

        var includeTestFakes = enumAttr.NamedArguments
            .FirstOrDefault(na => na.Key == "IncludeTestFakes").Value.Value is true;

        return new FeatureFlagsModel
        {
            EnumName = node.Identifier.Text,
            NamespaceName = namespaceName,
            FeatureFlagNames = featureFlagNames,
            IncludeTestFakes = includeTestFakes,
        };
    }

    private static ImmutableArray<string> GetFeatureFlagNames(INamedTypeSymbol enumSymbol)
    {
        var featureFlagMembers = enumSymbol.GetMembers();
        var featureFlagNames = featureFlagMembers
            .OfType<IFieldSymbol>()
            .Select(m => m.Name)
            .OrderBy(m => m, StringComparer.OrdinalIgnoreCase)
            .ToImmutableArray();
        return featureFlagNames;
    }

    private static AttributeData? GetFeatureAttribute(GeneratorSyntaxContext ctx, INamedTypeSymbol enumSymbol)
    {
        var ffAttr = ctx.SemanticModel.Compilation
            .GetTypeByMetadataName("Stravaig.FeatureFlags.StronglyTypedFeatureFlagsAttribute");
        if (ffAttr == null)
            return null;

        var enumAttrs = enumSymbol.GetAttributes();
        var enumAttr = enumAttrs.FirstOrDefault(a => ffAttr.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
        return enumAttr;
    }


    public bool Equals(FeatureFlagsModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return NamespaceName == other.NamespaceName && FeatureFlagNames.SequenceEqual(other.FeatureFlagNames) && IncludeTestFakes == other.IncludeTestFakes;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((FeatureFlagsModel) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = NamespaceName?.GetHashCode() ?? 0;
            for (int i = 0; i < FeatureFlagNames.Length; i++)
                hashCode = (hashCode * 397) ^ FeatureFlagNames[i].GetHashCode();
            hashCode = (hashCode * 397) ^ EnumName.GetHashCode();
            hashCode = (hashCode * 397) ^ IncludeTestFakes.GetHashCode();
            return hashCode;
        }
    }
}