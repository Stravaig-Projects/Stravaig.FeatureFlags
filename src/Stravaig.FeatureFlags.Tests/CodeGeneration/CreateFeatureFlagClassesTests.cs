namespace Stravaig.FeatureFlags.Tests.CodeGeneration;

[TestFixture]
public class CreateFeatureFlagClassesTests : VerifySourceGeneratorTests
{
    [Test]
    public async Task FileScopedNamespaceTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags]
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
using Stravaig.FeatureFlags;

namespace My.Test.Namespace
{
    [StronglyTypedFeatureFlags]
    public enum FeatureFlagNames
    {
        FeatureOne,
        FeatureTwo,
    }
}
";

        await VerifyGeneratedSource(source);
    }
    
    [Test]
    public async Task FullyQualifiesAttributeTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace
{
    [Stravaig.FeatureFlags.StronglyTypedFeatureFlagsAttribute]
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