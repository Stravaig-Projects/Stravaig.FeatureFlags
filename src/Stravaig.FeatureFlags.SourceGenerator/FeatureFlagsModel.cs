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
    internal string? NamespaceName { get; private init; }
    
    internal string EnumName { get; private init; }
    internal ImmutableArray<string> FeatureFlagNames { get; private init; }
    internal bool IncludeTestFakes { get; private init; }
    
    
    internal static FeatureFlagsModel? Create(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        var node = (EnumDeclarationSyntax)ctx.Node;
        var enumSymbol = ctx.SemanticModel.GetDeclaredSymbol(node, ct);
        if (enumSymbol == null)
            return null;
        
        var ffAttr = ctx.SemanticModel.Compilation
            .GetTypeByMetadataName("Stravaig.FeatureFlags.StronglyTypedFeatureFlagsAttribute");
        if (ffAttr == null)
            return null;

        var enumAttr = enumSymbol.GetAttributes()
            .FirstOrDefault(a => ffAttr.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
        if (enumAttr == null)
            return null;

        var featureFlagNames = enumSymbol.GetMembers()
            .Select(m => m.Name)
            .OrderBy(m => m, StringComparer.OrdinalIgnoreCase)
            .ToImmutableArray();

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
            var hashCode = NamespaceName.GetHashCode();
            for (int i = 0; i < FeatureFlagNames.Length; i++)
                hashCode = (hashCode * 397) ^ FeatureFlagNames[i].GetHashCode();
            hashCode = (hashCode * 397) ^ IncludeTestFakes.GetHashCode();
            return hashCode;
        }
    }
}