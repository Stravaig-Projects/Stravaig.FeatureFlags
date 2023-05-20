namespace Stravaig.FeatureFlags.Tests.CodeGeneration;

[TestFixture]
public class NamespaceTests : VerifySourceGeneratorTests
{
    [Test]
    public async Task FileScopedNamespaceTest()
    {
        const string source = @"
using Stravaig.FeatureFlagNames;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags]
public enum FeatureFlagNames
{
}
";

        await VerifyGeneratedSource(source);
    }
    
    [Test]
    public async Task BlockScopedNamespaceTest()
    {
        const string source = @"
using Stravaig.FeatureFlagNames;

namespace My.Test.Namespace
{
    [StronglyTypedFeatureFlags]
    public enum FeatureFlagNames
    {
    }
}
";

        await VerifyGeneratedSource(source);
    }
}