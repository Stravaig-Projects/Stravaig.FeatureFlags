namespace Stravaig.FeatureFlags.Tests;

[TestFixture]
public class SimpleTest : VerifySourceGeneratorTests
{
    [Test]
    public async Task HappyPathTest()
    {
        string source = @"
using Stravaig.FeatureFlags;

namespace MyTestNamespace;

[StronglyTypedFeatureFlags]
public enum FeatureFlags
{
    FeatureOne,
    FeatureTwo,
}
";

        await VerifyGeneratedSource(source);
    }
}