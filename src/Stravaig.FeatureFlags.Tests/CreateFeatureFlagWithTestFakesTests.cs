namespace Stravaig.FeatureFlags.Tests;

[TestFixture]
public class CreateFeatureFlagWithTestFakesTests : VerifySourceGeneratorTests
{
    [Test]
    public async Task FileScopedNamespaceTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(IncludeTestFakes = true)]
public enum FeatureFlags
{
    FeatureOne,
    FeatureTwo,
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
    [StronglyTypedFeatureFlags(IncludeTestFakes = true)]
    public enum FeatureFlags
    {
        FeatureOne,
        FeatureTwo,
    }
}
";

        await VerifyGeneratedSource(source);
    }
}