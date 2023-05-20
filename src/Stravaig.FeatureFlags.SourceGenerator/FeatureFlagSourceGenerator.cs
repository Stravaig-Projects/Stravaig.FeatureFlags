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
            .CreateSyntaxProvider(IsLikelyFeatureFlagEnum, CreateModel);
        
        context.RegisterSourceOutput(valueProvider, GenerateCode);
    }

    private static void GenerateCode(SourceProductionContext ctx, FeatureFlagsModel? model)
    {
        if (model != null)
            new SourceWriter().GenerateCode(ctx, model);
    }

    private static FeatureFlagsModel? CreateModel(GeneratorSyntaxContext ctx, CancellationToken ct) => FeatureFlagsModel.Create(ctx, ct);
    
    private static bool IsLikelyFeatureFlagEnum(
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

        // var attrLists = enumDeclaration.AttributeLists;
        // if (!attrLists.Any())
        //     return false;
        //
        // return attrLists
        //     .SelectMany(al => al.Attributes)
        //     .Any(a => ExtractName(a.Name) is "StronglyTypedFeatureFlagsAttribute" or "StronglyTypedFeatureFlags");
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