using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stravaig.FeatureFlags.SourceGenerator;

[StronglyTypedFeatureFlags, Flags]
[SuppressMessage("", "")]
public enum SampleFeatureFlags
{
    Alpha,
    
    [Lifetime(Lifetime.Transient)]
    Beta,
    
    [Lifetime(Lifetime.Scoped)]
    Gamma,
    
    [Lifetime(Lifetime.Singleton)]
    Delta,
}

[Generator]
public class FeatureFlagSourceGenerator : IIncrementalGenerator
{
    public class GeneratorContext
    {
        public EnumDeclarationSyntax EnumDeclaration { get; }
        public SemanticModel SemanticModel { get; }

        public TypeInfo GetEnumTypeInfo() => SemanticModel.GetTypeInfo(EnumDeclaration);
        
        public GeneratorContext(EnumDeclarationSyntax enumDeclaration, SemanticModel semanticModel)
        {
            EnumDeclaration = enumDeclaration;
            SemanticModel = semanticModel;
        }
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumTypes = context.SyntaxProvider
            .CreateSyntaxProvider(IsFeatureFlagEnum, (ctx, _) => new GeneratorContext((EnumDeclarationSyntax)ctx.Node, ctx.SemanticModel);
        
        context.RegisterSourceOutput(enumTypes, GenerateCode);
    }

    private static bool IsFeatureFlagEnum(
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken)
    {
        if (!syntaxNode.IsKind(SyntaxKind.EnumDeclaration))
            return false;

        var enumDeclaration = (EnumDeclarationSyntax)syntaxNode;

        var attributes = enumDeclaration.AttributeLists
            .SelectMany(al => al.Attributes).ToImmutableArray();
        if (attributes.Length == 0)
            return false;
        
        return attributes.Any(a => ExtractName(a.Name) is nameof(StronglyTypedFeatureFlagsAttribute) or "StronglyTypedFeatureFlags");
    }

    private static string? ExtractName(NameSyntax? name)
    {
        return name switch
        {
            SimpleNameSyntax ins => ins.Identifier.Text,
            QualifiedNameSyntax qns => qns.Right.Identifier.Text,
            _ => null
        };
    }

    private static void GenerateCode(
        SourceProductionContext productionContext,
        GeneratorContext generatorContext)
    {
        var enumType = generatorContext.GetEnumTypeInfo();
        var enumNamespace = enumType.Type?.ContainingNamespace;
        
        
        
        // foreach (var type in enumerations)
        // {
        //     var code = GenerateCode(type);
        //     var typeNamespace = type.ContainingNamespace.IsGlobalNamespace
        //         ? null
        //         : $"{type.ContainingNamespace}.";
        //
        //     productionContext.AddSource($"{typeNamespace}{type.Name}.g.cs", code);
        //
        //     var testCode = GenerateTestCode(type);
        //     productionContext.AddSource($"{typeNamespace}Testing.{type.Name}.g.cs", testCode);
        // }
    }

    private static string GenerateCode(ITypeSymbol type)
    {
        var ns = type.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.ContainingNamespace.ToString();
        var name = type.Name;
        var items = GetItemNames(type);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(@$"// <auto-generated />
using Stravaig.FeatureFlag;

{(ns is null ? null : $@"namespace {ns};")}
");

        foreach (var item in items)
        {
            sb.AppendLine(@$"public interface I{item}FeatureFlag : IStronglyTypedFeatureFlag
{{
}}

public sealed class {item}FeatureFlag : StronglyTypedFeatureFlag, I{item}FeatureFlag
{{
    public {item}FeatureFlat(IFeatureManager featureManager)
        : base(featureManager, ""{item}"")
    {{
    }}
}}
");
        }

        sb.AppendLine(@$"
public static class {name}ServiceExtensions
{{
    public static IFeatureManagementBuilder AddStronglyTypedFlags(this IFeatureManagementBuilder builder)
    {{");
        foreach (var item in items)
        {
            sb.AppendLine($"        builder.Services.AddScoped<I{item}FeatureFlag, {item}FeatureFlag>();");
        }

        sb.AppendLine($@"        return builder;
    }}
}}");

        return sb.ToString();
    }

    private static string GenerateTestCode(ITypeSymbol type)
    {
        var ns = type.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.ContainingNamespace.ToString();
        var items = GetItemNames(type);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(@$"// <auto-generated />
using Stravaig.FeatureFlag;

{(ns is null ? null : $@"namespace {ns}.Testing;")}
");

        foreach (var item in items)
        {
            sb.AppendLine(@$"
public sealed class Fake{item}FeatureFlag : I{item}FeatureFlag
{{
    public static readonly Fake{item}FeatureFlag Enabled = new (true);
    public static readonly Fake{item}FeatureFlag Disabled = new (false);

    private readonly bool _state;
    public Fake{item}FeatureFlag(bool state)
    {{
        _state = state;
    }}

    public Task<bool> IsEnabledAsync() => Task.FromResult(_state);

    public bool IsEnabled() => _state;
}}
");
        }
        return sb.ToString();
    }


    private static IReadOnlyList<string> GetItemNames(ITypeSymbol type)
    {
        return type.GetMembers()
            .Select(m =>
            {
                if (!m.IsStatic ||
                    m.DeclaredAccessibility != Accessibility.Public ||
                    m is not IFieldSymbol field)
                {
                    return null;
                }

                return SymbolEqualityComparer.Default.Equals(field.Type, type)
                    ? field.Name
                    : null;
            })
            .Where(field => field is not null)
            .Cast<string>()
            .ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase);
    }
}