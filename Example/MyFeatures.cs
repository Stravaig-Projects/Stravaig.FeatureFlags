using Stravaig.FeatureFlags;

namespace Example;

[StronglyTypedFeatureFlags(IncludeTestFakes = true)]
public enum MyFeatures
{
    Alpha,
    Beta,
    Gamma,
}

