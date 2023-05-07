using Stravaig.FeatureFlags;

namespace Example;

[StronglyTypedFeatureFlags(IncludeTestFakes = true, DefaultLifetime = Lifetime.Transient)]
public enum MyFeatures
{
    Alpha,
    
    [Lifetime(Lifetime.Transient)]
    Beta,
    Gamma,
}

