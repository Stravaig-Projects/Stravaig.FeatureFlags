using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stravaig.FeatureFlags.SourceGenerator;

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
        
        return attributes.Any(a => ExtractName(a.Name) is "StronglyTypedFeatureFlagsAttribute" or "StronglyTypedFeatureFlags");
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
}