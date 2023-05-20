// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.FeatureManagement;
// using Shouldly;
// // ReSharper disable MethodHasAsyncOverload
//
// namespace Stravaig.FeatureFlags.Tests.DependencyInjection;
//
// [StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Singleton)]
// public enum DependencyInjectionTestsFeatureFlags
// {
//     Alpha,
//
//     [Lifetime(Lifetime.Scoped)]
//     Beta,
//
//     [Lifetime(Lifetime.Transient)]
//     Gamma,
// }
//
// [TestFixture]
// public class DependencyInjectionTests
// {
//     [Test]
//     public async Task EnsureFlagsAreAddedToContainerAndCanBeResolved()
//     {
//         // Arrange
//         var (_, provider) = BuildContainer(new[]
//         {
//             new KeyValuePair<string, string>("FeatureManagement:Alpha", "true"),
//             new KeyValuePair<string, string>("FeatureManagement:Beta", "false"),
//         });
//
//         // Assert
//         var alpha = provider.GetRequiredService<IAlphaFeatureFlag>();
//         (await alpha.IsEnabledAsync()).ShouldBeTrue();
//         alpha.IsEnabled().ShouldBeTrue();
//         
//         var beta = provider.GetRequiredService<IBetaFeatureFlag>();
//         (await beta.IsEnabledAsync()).ShouldBeFalse();
//         beta.IsEnabled().ShouldBeFalse();
//
//         var gamma = provider.GetRequiredService<IGammaFeatureFlag>();
//         (await gamma.IsEnabledAsync()).ShouldBeFalse();
//         gamma.IsEnabled().ShouldBeFalse();
//     }
//
//     [Test]
//     public async Task EnsureTransientFlagsGetUpdatedValueOnEachResolution()
//     {
//         // Arrange
//         var (configRoot, provider) = BuildContainer(new[]
//         {
//             new KeyValuePair<string, string>("FeatureManagement:Gamma", "true"),
//         });
//         
//         // Act & Assert
//         var gamma1 = provider.GetRequiredService<IGammaFeatureFlag>();
//         (await gamma1.IsEnabledAsync()).ShouldBeTrue();
//         gamma1.IsEnabled().ShouldBeTrue();
//         
//         configRoot.Providers.First().Set("FeatureManagement:Gamma", "false");
//         configRoot.Reload();
//         var gamma2 = provider.GetRequiredService<IGammaFeatureFlag>();
//         gamma1.ShouldNotBe(gamma2);
//         (await gamma2.IsEnabledAsync()).ShouldBeFalse();
//         gamma2.IsEnabled().ShouldBeFalse();
//     }
//     
//     [Test]
//     public async Task EnsureScopedFlagsGetUpdatedValueOnNewScope()
//     {
//         // Arrange
//         var (configRoot, provider) = BuildContainer(new[]
//         {
//             new KeyValuePair<string, string>("FeatureManagement:Beta", "true"),
//         });
//
//         IBetaFeatureFlag beta1, beta2;
//         // Act & Assert
//         using (var scope1 = provider.CreateScope())
//         {
//             beta1 = scope1.ServiceProvider.GetRequiredService<IBetaFeatureFlag>();
//             (await beta1.IsEnabledAsync()).ShouldBeTrue();
//             beta1.IsEnabled().ShouldBeTrue();
//             
//             configRoot.Providers.First().Set("FeatureManagement:Beta", "false");
//             configRoot.Reload();
//
//             // Even altho' the config value has changed, resolving beta2 should
//             // bring back the same object as beta1 and thus have the same value.
//             beta2 = scope1.ServiceProvider.GetRequiredService<IBetaFeatureFlag>();
//             beta1.ShouldBe(beta2);
//             (await beta2.IsEnabledAsync()).ShouldBeTrue();
//             beta2.IsEnabled().ShouldBeTrue();
//         }
//
//         using (var scope2 = provider.CreateScope())
//         {
//             // This is a new scope, so when beta3 is resolved, it should not be
//             // the same as beta1 or beta2 and it should have the new value.
//             var beta3 = scope2.ServiceProvider.GetRequiredService<IBetaFeatureFlag>();
//             beta1.ShouldNotBe(beta3);
//             beta2.ShouldNotBe(beta3);
//             (await beta3.IsEnabledAsync()).ShouldBeFalse();
//             beta3.IsEnabled().ShouldBeFalse();
//         }
//     }
//     
//     [Test]
//     public async Task EnsureSingletonFlagsKeepOriginalValue()
//     {
//         // Arrange
//         var (configRoot, provider) = BuildContainer(new[]
//         {
//             new KeyValuePair<string, string>("FeatureManagement:Alpha", "true"),
//         });
//
//         // Act & Assert
//         var alpha1 = provider.GetRequiredService<IAlphaFeatureFlag>();
//         (await alpha1.IsEnabledAsync()).ShouldBeTrue();
//         alpha1.IsEnabled().ShouldBeTrue();
//
//         configRoot.Providers.First().Set("FeatureManagement:Alpha", "false");
//         configRoot.Reload();
//
//         var alpha2 = provider.GetRequiredService<IAlphaFeatureFlag>();
//         alpha2.ShouldBe(alpha1);
//         (await alpha2.IsEnabledAsync()).ShouldBeTrue();
//         alpha2.IsEnabled().ShouldBeTrue();
//     }
//     
//     private static (IConfigurationRoot configRoot, ServiceProvider provider) BuildContainer(KeyValuePair<string, string>[] configValues)
//     {
//         var configBuilder = new ConfigurationBuilder();
//         configBuilder.AddInMemoryCollection(configValues);
//         var configRoot = configBuilder.Build();
//
//         var services = new ServiceCollection();
//
//         services.AddFeatureManagement(configRoot)
//             .SetupStronglyTypedFeatures(opts => { opts.Add<DependencyInjectionTestsFeatureFlags>(); });
//         var provider = services.BuildServiceProvider();
//         return (configRoot, provider);
//     }
// }