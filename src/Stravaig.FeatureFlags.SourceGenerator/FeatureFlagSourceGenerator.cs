using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
            .CreateSyntaxProvider(IsLikelyFeatureFlagEnum, CreateSourceWriter);
        
        context.RegisterSourceOutput(valueProvider, GenerateCode);
    }

    private static void GenerateCode(SourceProductionContext ctx, SourceWriter writer) => writer.GenerateCode(ctx);    

    private static SourceWriter CreateSourceWriter(GeneratorSyntaxContext ctx, CancellationToken _)
        => new ((EnumDeclarationSyntax)ctx.Node);
    
    private static bool IsLikelyFeatureFlagEnum(
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken)
    {
        if (!syntaxNode.IsKind(SyntaxKind.EnumDeclaration))
            return false;

        var enumDeclaration = (EnumDeclarationSyntax)syntaxNode;

        var attrLists = enumDeclaration.AttributeLists;
        if (!attrLists.Any())
            return false;

        return attrLists
            .SelectMany(al => al.Attributes)
            .Any(a => ExtractName(a.Name) is "StronglyTypedFeatureFlagsAttribute" or "StronglyTypedFeatureFlags");
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