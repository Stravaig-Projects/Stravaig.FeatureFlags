using System;

namespace Stravaig.FeatureFlags;

[AttributeUsage(AttributeTargets.Enum)]
public sealed class StronglyTypedFeatureFlagsAttribute : Attribute
{
    public Lifetime DefaultLifetime { get; }

    public StronglyTypedFeatureFlagsAttribute(Lifetime defaultLifetime = Lifetime.Scoped)
    {
        DefaultLifetime = defaultLifetime;
    }
}