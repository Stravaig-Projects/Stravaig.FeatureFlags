using System;

namespace Stravaig.FeatureFlags;

[AttributeUsage(AttributeTargets.Enum)]
public sealed class StronglyTypedFeatureFlagsAttribute : Attribute
{
    public Lifetime DefaultLifetime { get; set; }

    public StronglyTypedFeatureFlagsAttribute()
    {
        DefaultLifetime = Lifetime.Scoped;
    }
}