using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stravaig.FeatureFlags.SourceGenerator;

public class SourceWriter
{
    private const string TransientLifetime = "Transient";
    private const string ScopedLifetime = "Scoped";
    private const string SingletonLifetime = "Singleton";
    private const string DefaultLifetime = ScopedLifetime;
    private static readonly ImmutableArray<string> ValidLifetimes =
        ImmutableArray.Create<string>(ScopedLifetime, TransientLifetime, SingletonLifetime);
    
    
    private readonly EnumDeclarationSyntax _enumDeclaration;
    private readonly SemanticModel _semanticModel;
    
    //private TypeInfo GetEnumTypeInfo(CancellationToken ct = default) => _semanticModel.GetTypeInfo(_enumDeclaration, ct);
        
    //private CompilationUnitSyntax GetRoot(CancellationToken ct = default) => (CompilationUnitSyntax)_enumDeclaration.SyntaxTree.GetRoot(ct);

    private BaseNamespaceDeclarationSyntax? GetNamespaceOfEnum() =>
        _enumDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

    private SyntaxKind? GetNamespaceKind() =>
        GetNamespaceOfEnum()?.Kind();

    private StringResult GetDefaultLifetime()
    {
        var attr = _enumDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .First(a => ExtractName(a.Name) is "StronglyTypedFeatureFlagsAttribute" or "StronglyTypedFeatureFlags");

        var defaultLifetimeArg =
            attr.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.ToString() == "DefaultLifetime");
        if (defaultLifetimeArg == null)
            return new(_enumDeclaration, DefaultLifetime);

        var value = ((MemberAccessExpressionSyntax)defaultLifetimeArg.Expression).Name.ToString();
        if (ValidLifetimes.Contains(value, StringComparer.Ordinal))
            return new(defaultLifetimeArg, value);

        return new(defaultLifetimeArg, DefaultLifetime, $"Lifetime of \"{value}\" specified is invalid. Using {DefaultLifetime}.");
    }

    private bool IncludeTestFakes()
    {
        var attr = _enumDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .First(a => ExtractName(a.Name) is "StronglyTypedFeatureFlagsAttribute" or "StronglyTypedFeatureFlags");

        var testFakesArg =
            attr.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.ToString() == "IncludeTestFakes");
        if (testFakesArg == null)
            return false;


        switch (testFakesArg.Expression.Kind())
        {
            case SyntaxKind.TrueLiteralExpression:
                return true;
            case SyntaxKind.FalseLiteralExpression:
                return false;
            default:
                // TODO: process other possibilities.
                return false;
        }
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

        bool includeFakes = IncludeTestFakes();
        if (!includeFakes)
            return;

        fileContent.Clear();
        AddFileHeader(fileContent, isTesting: true);
        indent = AddNamespaceStart(fileContent, namespaceName, isTesting: true);
        AddTestingClasses(fileContent, indent);
        AddNamespaceEnd(fileContent, namespaceName);
        WriteFeatureFlagClassesFile(productionContext, namespaceName, fileContent, isTesting: true);
    }

    private void AddTestingClasses(StringBuilder fileContent, string indent)
    {
        var enumMembers = _enumDeclaration.Members;
        if (enumMembers.Count == 0)
            return;

        foreach (var item in enumMembers)
        {
            string flagName = item.Identifier.Text;
            fileContent.AppendLine(@$"
{indent}public sealed class Fake{flagName}FeatureFlag : Stravaig.FeatureFlags.Testing.FakeFeatureFlag, I{flagName}FeatureFlag
{indent}{{
{indent}    public static readonly Fake{flagName}FeatureFlag Enabled = new Fake{flagName}FeatureFlag(true);
{indent}    public static readonly Fake{flagName}FeatureFlag Disabled = new Fake{flagName}FeatureFlag(false);

{indent}    public Fake{flagName}FeatureFlag(bool state) : base(state)
{indent}    {{
{indent}    }}
{indent}}}
");
        }
    }

    private void AddBuilderClass(StringBuilder fileContent, string indent)
    {
        var enumMembers = _enumDeclaration.Members;
        if (enumMembers.Count == 0)
            return;

        var defaultLifetime = GetDefaultLifetime();
        string enumName = _enumDeclaration.Identifier.Text;
        fileContent.AppendLine(@$"
{indent}public static class {enumName}ServiceExtensions
{indent}{{
{indent}    public static IFeatureManagementBuilder AddStronglyTyped{enumName}(this IFeatureManagementBuilder builder)
{indent}    {{");
        foreach (var item in enumMembers)
        {
            var lifetime = GetFeatureFlagLifetime(item, defaultLifetime);
            if (lifetime.HasError)
                fileContent.AppendLine($"#error {lifetime.ErrorWithPosition}");
            string flagName = item.Identifier.Text;
            fileContent.AppendLine($"{indent}        builder.Services.Add{lifetime.Value}<I{flagName}FeatureFlag, {flagName}FeatureFlag>();");
        }

        fileContent.AppendLine($@"{indent}        return builder;
{indent}    }}
{indent}}}");
    }

    private StringResult GetFeatureFlagLifetime(EnumMemberDeclarationSyntax enumMember, StringResult defaultLifetime)
    {
        var lifetimeAttribute = enumMember.AttributeLists
            .SelectMany(al => al.Attributes)
            .FirstOrDefault(a => a.Name.ToString() == "Lifetime");

        if (lifetimeAttribute == null)
            return defaultLifetime;

        var arg = lifetimeAttribute.ArgumentList?.Arguments.FirstOrDefault();
        if (arg == null)
            return defaultLifetime;

        if (arg.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
        {
            string value = ((MemberAccessExpressionSyntax)arg.Expression).Name.ToString();
            if (!ValidLifetimes.Contains(value, StringComparer.Ordinal))
                return new(arg, DefaultLifetime, $"Lifetime of \"{value}\" specified is invalid. Using {DefaultLifetime}.");
            return new (arg, value);
        }

        return new(arg, DefaultLifetime,
            $"Cannot interpret \"{arg.Expression.ToString()}\" in this version of the source generator. Please use \"Lifetime.<value>\".");
    }

    private void AddStronglyTypedFeatureFlagClasses(StringBuilder fileContent, string indent)
    {
        var enumMembers = _enumDeclaration.Members;

        foreach (var item in enumMembers)
        {
            string name = item.Identifier.Text;
            fileContent.AppendLine(@$"{indent}public interface I{name}FeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{indent}{{
{indent}}}

{indent}public sealed class {name}FeatureFlag : Stravaig.FeatureFlags.FeatureFlag, I{name}FeatureFlag
{indent}{{
{indent}    public {name}FeatureFlag(IFeatureManager featureManager)
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
        StringBuilder fileContent,
        bool isTesting = false)
    {
        var testing = isTesting ? ".Testing" : string.Empty;
        var enumName = _enumDeclaration.Identifier.Text;
        var fileName = $"{namespaceName ?? "global."}{testing}.{enumName}.g.cs";
        var source = fileContent.ToString();
        productionContext.AddSource(fileName, source);
    }

    private void AddFileHeader(StringBuilder fileContent, bool isTesting = false)
    {
        fileContent.AppendLine("// <auto-generated />");
        fileContent.AppendLine();
        if (isTesting == false)
            fileContent.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        fileContent.AppendLine("using Microsoft.FeatureManagement;");
        fileContent.AppendLine("using Stravaig.FeatureFlags;");
        fileContent.AppendLine();
    }

    private string AddNamespaceStart(StringBuilder fileContent, string? namespaceName, bool isTesting = false)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
            return string.Empty;

        fileContent.Append($"namespace {namespaceName}");
        if (isTesting)
            fileContent.Append(".Testing");

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