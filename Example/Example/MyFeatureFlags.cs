using Microsoft.AspNetCore.Routing.Constraints;
using Stravaig.FeatureFlags;

namespace Example;

[StronglyTypedFeatureFlags]
public enum MyFeatureFlags
{
    [Lifetime(Lifetime.Singleton)]
    Alpha,
    Beta,
    Gamma,
}