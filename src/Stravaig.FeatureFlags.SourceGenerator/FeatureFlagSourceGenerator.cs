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
using System.Xml.Linq;

namespace Stravaig.FeatureFlags.SourceGenerator;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Transient)]
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


public class SourceWriter
{
    private readonly EnumDeclarationSyntax _enumDeclaration;
    private readonly SemanticModel _semanticModel;
    
    //private TypeInfo GetEnumTypeInfo(CancellationToken ct = default) => _semanticModel.GetTypeInfo(_enumDeclaration, ct);
        
    //private CompilationUnitSyntax GetRoot(CancellationToken ct = default) => (CompilationUnitSyntax)_enumDeclaration.SyntaxTree.GetRoot(ct);

    private BaseNamespaceDeclarationSyntax? GetNamespaceOfEnum() =>
        _enumDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

    private SyntaxKind? GetNamespaceKind() =>
        GetNamespaceOfEnum()?.Kind();

    private string GetDefaultLifetime()
    {
        var attr = _enumDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .First(a => ExtractName(a.Name) is nameof(StronglyTypedFeatureFlagsAttribute) or "StronglyTypedFeatureFlags");

        var defaultLifetimeArg =
            attr.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.ToString() == "DefaultLifetime");
        if (defaultLifetimeArg == null)
            return "Scoped";

        return ((MemberAccessExpressionSyntax)defaultLifetimeArg.Expression).Name.ToString();
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

    public SourceWriter(EnumDeclarationSyntax enumDeclaration, SemanticModel semanticModel)
    {
        _enumDeclaration = enumDeclaration;
        _semanticModel = semanticModel;
    }
    
    public void GenerateCode(SourceProductionContext productionContext)
    {
        var enumNamespace = GetNamespaceOfEnum();
        var namespaceName = enumNamespace?.Name.ToString();
        
        StringBuilder fileContent = new StringBuilder();
        AddFileHeader(fileContent);
        string indent = AddNamespaceStart(fileContent, namespaceName);

        AddStronglyTypedFeatureFlagClasses(fileContent, indent);

        AddBuilderClass(fileContent, indent);

        AddNamespaceEnd(fileContent, namespaceName);

        WriteFeatureFlagClassesFile(productionContext, namespaceName, fileContent);

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

    private void AddBuilderClass(StringBuilder fileContent, string indent)
    {
        var enumMembers = _enumDeclaration.Members;
        if (enumMembers.Count == 0)
            return;

        string defaultLifetime = GetDefaultLifetime();
        string enumName = _enumDeclaration.Identifier.Text;
        fileContent.AppendLine(@$"
{indent}public static class {enumName}ServiceExtensions
{indent}{{
{indent}    public static IFeatureManagementBuilder AddStronglyTyped{enumName}(this IFeatureManagementBuilder builder)
{indent}    {{");
        foreach (var item in enumMembers)
        {
            var lifetime = defaultLifetime;
            string flagName = item.Identifier.Text;
            fileContent.AppendLine($"{indent}        builder.Services.Add{lifetime}<I{flagName}FeatureFlag, {flagName}FeatureFlag>();");
        }

        fileContent.AppendLine($@"{indent}        return builder;
{indent}    }}
{indent}}}");
    }

    private void AddStronglyTypedFeatureFlagClasses(StringBuilder fileContent, string indent)
    {
        var enumMembers = _enumDeclaration.Members;

        foreach (var item in enumMembers)
        {
            string name = item.Identifier.Text;
            fileContent.AppendLine(@$"{indent}public interface I{name}FeatureFlag : IStronglyTypedFeatureFlag
{indent}{{
{indent}}}

{indent}public sealed class {name}FeatureFlag : StronglyTypedFeatureFlag, I{name}FeatureFlag
{indent}{{
{indent}    public {name}FeatureFlat(IFeatureManager featureManager)
{indent}        : base(featureManager, ""{name}"")
{indent}    {{
{indent}    }}
{indent}}}
");
        }
    }

    private void WriteFeatureFlagClassesFile(
        SourceProductionContext productionContext,
        string? namespaceName,
        StringBuilder fileContent)
    {
        var enumName = _enumDeclaration.Identifier.Text;
        var fileName = $"{namespaceName ?? "global."}.{enumName}.g.cs";
        var source = fileContent.ToString();
        productionContext.AddSource(fileName, source);
    }

    private void AddFileHeader(StringBuilder fileContent)
    {
        fileContent.AppendLine("// <auto-generated />");
        fileContent.AppendLine();
        fileContent.AppendLine("using Stravaig.FeatureFlag;");
        fileContent.AppendLine();
    }

    private string AddNamespaceStart(StringBuilder fileContent, string? namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
            return string.Empty;

        fileContent.Append($"namespace {namespaceName}");

        var kind = GetNamespaceKind();
        if (kind == SyntaxKind.FileScopedNamespaceDeclaration)
        {
            fileContent.AppendLine(";");
            return string.Empty;
        }

        fileContent.AppendLine();
        fileContent.AppendLine("{");
        return "    "; // indent for the rest of the code.
    }

    private void AddNamespaceEnd(StringBuilder fileContent, string? namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
            return;

        var kind = GetNamespaceKind();
        if (kind == SyntaxKind.NamespaceDeclaration)
            fileContent.AppendLine("}");
    }
}

[Generator]
public class FeatureFlagSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var valueProvider = context.SyntaxProvider
            .CreateSyntaxProvider(IsFeatureFlagEnum, CreateSourceWriter);
        
        context.RegisterSourceOutput(valueProvider, GenerateCode);
    }

    private static void GenerateCode(SourceProductionContext ctx, SourceWriter writer) => writer.GenerateCode(ctx);    

    private static SourceWriter CreateSourceWriter(GeneratorSyntaxContext ctx, CancellationToken _)
        => new ((EnumDeclarationSyntax)ctx.Node, ctx.SemanticModel);
    
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