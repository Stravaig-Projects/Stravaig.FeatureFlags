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

    [Test]
    public async Task JustDefaultNonsenseLifetimeTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Nonsense)]
public enum FeatureFlags
{
    FeatureOne,
    FeatureTwo,
}
";

        await VerifyGeneratedSource(source);
    }
    
    [Test]
    public async Task JustDefaultNonLiteralTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(DefaultLifetime = Nonsense)]
public enum FeatureFlags
{
    FeatureOne,
    FeatureTwo,
}
";

        await VerifyGeneratedSource(source);
    }
    
    [Test]
    public async Task SpecificNonsenseLifetimeTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Singleton)]
public enum FeatureFlags
{
    Alpha,

    [Lifetime(Lifetime.Garbage)]
    Beta,

    [Lifetime(Lifetime.Rubbish)]
    Gamma,

    [Lifetime(Lifetime.Trash)]
    Delta,

    [Lifetime(Lifetime.)]
    Epsilon,

    [Lifetime(Lifetime)]
    Zeta,

    [Lifetime(eta)]
    Eta,

    [Lifetime((Lifetime)1)]
    Theta,
}
";

        await VerifyGeneratedSource(source);
    }
    
    [Test]
    public async Task DefaultAndSpecificLifetimeTest()
    {
        const string source = @"
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Scoped)]
public enum FeatureFlags
{
    [Lifetime(Lifetime.Transient)]
    TheTransientFeature,

    [Lifetime(Lifetime.Scoped)]
    TheScopedFeature,

    [Lifetime(Lifetime.Singleton)]
    TheSingletonFeature,

    TheDefaultFeature,
}
";

        await VerifyGeneratedSource(source);
    }
}