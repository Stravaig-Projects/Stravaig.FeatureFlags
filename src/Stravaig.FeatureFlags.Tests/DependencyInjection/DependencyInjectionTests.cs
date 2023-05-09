using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Shouldly;

namespace Stravaig.FeatureFlags.Tests.DependencyInjection;

[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Singleton)]
public enum DependencyInjectionTestsFeatureFlags
{
    Alpha,

    [Lifetime(Lifetime.Scoped)]
    Beta,

    [Lifetime(Lifetime.Transient)]
    Gamma,
}

[TestFixture]
public class DependencyInjectionTests
{
    [Test]
    public async Task Test()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("FeatureManagement:Alpha", "true"),
        });
        var configRoot = configBuilder.Build();

        var services = new ServiceCollection();
        services.AddFeatureManagement(configRoot)
            .SetupStronglyTypedFeatures(opts =>
            {
                opts.Add<DependencyInjectionTestsFeatureFlags>();
            });
        var provider = services.BuildServiceProvider();

        var ff = provider.GetRequiredService<IAlphaFeatureFlag>();

        (await ff.IsEnabledAsync()).ShouldBeTrue();
        // ReSharper disable once MethodHasAsyncOverload
        ff.IsEnabled().ShouldBeTrue();
    }
}