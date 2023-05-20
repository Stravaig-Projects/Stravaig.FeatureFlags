namespace Stravaig.FeatureFlags.Tests.CodeGeneration;

[TestFixture]
public class CreateFeatureFlagWithTestFakesTests : VerifySourceGeneratorTests
{
    [Test]
    public async Task FileScopedNamespaceTest()
    {
        const string source = @"
using Stravaig.FeatureFlagNames;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(IncludeTestFakes = true)]
public enum FeatureFlagNames
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
using Stravaig.FeatureFlagNames;

namespace My.Test.Namespace
{
    [StronglyTypedFeatureFlags(IncludeTestFakes = true)]
    public enum FeatureFlagNames
    {
        FeatureOne,
        FeatureTwo,
    }
}
";

        await VerifyGeneratedSource(source);
    }
}