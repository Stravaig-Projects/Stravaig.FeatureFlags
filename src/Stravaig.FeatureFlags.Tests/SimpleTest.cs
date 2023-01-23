namespace Stravaig.FeatureFlags.Tests;

[TestFixture]
public class SimpleTest : VerifySourceGeneratorTests
{
    [Test]
    public async Task HappyPathTest()
    {
        string source = @"
using Stravaig.FeatureFlags;

[StronglyTypedFeatureFlags]
public enum FeatureFlags
{
    MyFirstFeature,
}
";

        await VerifyGeneratedSource(source);
    }
}