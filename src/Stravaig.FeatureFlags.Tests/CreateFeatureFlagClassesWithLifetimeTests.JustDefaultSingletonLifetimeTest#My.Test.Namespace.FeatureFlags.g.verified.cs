﻿//HintName: My.Test.Namespace.FeatureFlags.g.cs
// <auto-generated />

using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;
public interface IFeatureOneFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class FeatureOneFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IFeatureOneFeatureFlag
{
    public FeatureOneFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "FeatureOne")
    {
    }
}

public interface IFeatureTwoFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class FeatureTwoFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IFeatureTwoFeatureFlag
{
    public FeatureTwoFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "FeatureTwo")
    {
    }
}


public static class FeatureFlagsServiceExtensions
{
    public static IFeatureManagementBuilder AddStronglyTypedFeatureFlags(this IFeatureManagementBuilder builder)
    {
        builder.Services.AddSingleton<IFeatureOneFeatureFlag, FeatureOneFeatureFlag>();
        builder.Services.AddSingleton<IFeatureTwoFeatureFlag, FeatureTwoFeatureFlag>();
        return builder;
    }
}
