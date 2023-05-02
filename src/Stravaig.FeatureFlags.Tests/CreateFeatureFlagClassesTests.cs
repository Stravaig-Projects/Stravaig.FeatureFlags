namespace Stravaig.FeatureFlags.Tests;

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
    [StronglyTypedFeatureFlags]
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