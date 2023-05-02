namespace Stravaig.FeatureFlags.Tests;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Transient)]
public enum SampleFeatureFlags
{
    Alpha,
    
    [Lifetime(Lifetime.Transient)]
    Beta,
    
    [Lifetime(Lifetime.Scoped)]
    Gamma,
    
    [Lifetime(Lifetime.Singleton)]
    Delta,
}

[TestFixture]
public class CreateFeatureFlagClassesWithLifetimeTests : VerifySourceGeneratorTests
{
    [Test]
    public async Task JustDefaultTransientLifetimeTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Transient)]
public enum FeatureFlags
{
    FeatureOne,
    FeatureTwo,
}
";

        await VerifyGeneratedSource(source);
    }

    [Test]
    public async Task JustDefaultSingletonLifetimeTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Singleton)]
public enum FeatureFlags
{
    FeatureOne,
    FeatureTwo,
}
";

        await VerifyGeneratedSource(source);
    }

    [Test]
    public async Task JustDefaultScopedLifetimeTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Scoped)]
public enum FeatureFlags
{
    FeatureOne,
    FeatureTwo,
}
";

        await VerifyGeneratedSource(source);
    }
}