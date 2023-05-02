namespace Stravaig.FeatureFlags.Tests;

[TestFixture]
public class NamespaceTests : VerifySourceGeneratorTests
{
    [Test]
    public async Task FileScopedNamespaceTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags]
public enum FeatureFlags
{
}
";

        await VerifyGeneratedSource(source);
    }
    
    [Test]
    public async Task BlockScopedNamespaceTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace
{
    [StronglyTypedFeatureFlags]
    public enum FeatureFlags
    {
    }
}
";

        await VerifyGeneratedSource(source);
    }
}