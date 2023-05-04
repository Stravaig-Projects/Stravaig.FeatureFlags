﻿//HintName: My.Test.Namespace.FeatureFlags.g.cs
// <auto-generated />

using Microsoft.FeatureManagement;
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;
public interface IFeatureOneFeatureFlag : IStronglyTypedFeatureFlag
{
}

public sealed class FeatureOneFeatureFlag : StronglyTypedFeatureFlag, IFeatureOneFeatureFlag
{
    public FeatureOneFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "FeatureOne")
    {
    }
}

public interface IFeatureTwoFeatureFlag : IStronglyTypedFeatureFlag
{
}

public sealed class FeatureTwoFeatureFlag : StronglyTypedFeatureFlag, IFeatureTwoFeatureFlag
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
