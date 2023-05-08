using System;
using Microsoft.FeatureManagement;

namespace Stravaig.FeatureFlags;

[AttributeUsage(AttributeTargets.Enum)]
public sealed class StronglyTypedFeatureFlagsAttribute : Attribute
{
    /// <summary>
    /// The default lifetime of the feature flags.
    /// </summary>
    public Lifetime DefaultLifetime { get; set; }

    /// <summary>
    /// The generator should also generate Fake FeatureFlag objects in a subordinate "Testing" namespace.
    /// </summary>
    public bool IncludeTestFakes { get; set; }

    /// <summary>
    /// Initialises the StronglyTypeFeatureFlag attribute.
    /// </summary>
    public StronglyTypedFeatureFlagsAttribute()
    {
        DefaultLifetime = Lifetime.Scoped;
    }
}