using System;

namespace Stravaig.FeatureFlags;

[AttributeUsage(AttributeTargets.Field)]
public sealed class LifetimeAttribute : Attribute
{
    public Lifetime Lifetime { get; }

    public LifetimeAttribute(Lifetime lifetime)
    {
        Lifetime = lifetime;
    }
}