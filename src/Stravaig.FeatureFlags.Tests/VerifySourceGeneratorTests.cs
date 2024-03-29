using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stravaig.FeatureFlags.SourceGenerator;

namespace Stravaig.FeatureFlags.Tests;

public class VerifySourceGeneratorTests
{
    protected async Task VerifyGeneratedSource(string source, [CallerFilePath]string sourceFile = "")
    {
        Compilation inputCompilation = CreateCompilation(source);

        // directly create an instance of the generator
        // (Note: in the compiler this is loaded from an assembly, and created via reflection at runtime)
        FeatureFlagSourceGenerator generator = new FeatureFlagSourceGenerator();

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        //// We can now assert things about the resulting compilation:
        //Debug.Assert(diagnostics.IsEmpty); // there were no diagnostics created by the generators
        //Debug.Assert(outputCompilation.SyntaxTrees.Count() == 2); // we have two syntax trees, the original 'user' provided one, and the one added by the generator
        //Debug.Assert(outputCompilation.GetDiagnostics().IsEmpty); // verify the compilation with the added source has no diagnostics

        if (!diagnostics.IsEmpty)
        {
            var diagnosticMessages = string.Join(
                Environment.NewLine,
                diagnostics.Select(
                    d => d.ToString() +
                         Environment.NewLine +
                         string.Join(
                             Environment.NewLine,
                             d.Properties.Select(p => $" >> {p.Key} : {p.Value}") +
                             Environment.NewLine)));
            
            Assert.Fail(diagnosticMessages);
            return;
        }
        
        // Or we can look at the results directly:
        GeneratorDriverRunResult runResult = driver.GetRunResult();

        //// The runResult contains the combined results of all generators passed to the driver
        //Debug.Assert(runResult.GeneratedTrees.Length == 1);
        //Debug.Assert(runResult.Diagnostics.IsEmpty);

        //// Or you can access the individual results on a by-generator basis
        //GeneratorRunResult generatorResult = runResult.Results[0];
        //Debug.Assert(generatorResult.Generator == generator);
        //Debug.Assert(generatorResult.Diagnostics.IsEmpty);
        //Debug.Assert(generatorResult.GeneratedSources.Length == 1);
        //Debug.Assert(generatorResult.Exception is null);

        // ReSharper disable once ExplicitCallerInfoArgument
        await Verifier.Verify(runResult, sourceFile: sourceFile);
    }
    
    private static readonly string DotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

    private static readonly ImmutableArray<MetadataReference> References = ImmutableArray.Create<MetadataReference>(
        // .NET assemblies are finicky and need to be loaded in a special way.
        MetadataReference.CreateFromFile(Path.Combine(DotNetAssemblyPath, "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(DotNetAssemblyPath, "System.dll")),
        MetadataReference.CreateFromFile(Path.Combine(DotNetAssemblyPath, "System.Core.dll")),
        MetadataReference.CreateFromFile(Path.Combine(DotNetAssemblyPath, "System.Private.CoreLib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(DotNetAssemblyPath, "System.Runtime.dll")),
        MetadataReference.CreateFromFile(typeof(StronglyTypedFeatureFlagsAttribute).GetTypeInfo().Assembly.Location)
    );
    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source) },
            References,
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
}