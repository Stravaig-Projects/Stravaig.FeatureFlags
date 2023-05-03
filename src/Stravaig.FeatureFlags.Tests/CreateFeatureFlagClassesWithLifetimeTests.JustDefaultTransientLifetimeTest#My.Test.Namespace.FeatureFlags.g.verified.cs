﻿//HintName: My.Test.Namespace.FeatureFlags.g.cs
// <auto-generated />

using Microsoft.FeatureManagement;
using Stravaig.FeatureFlag;

namespace My.Test.Namespace;
public interface IFeatureOneFeatureFlag : IStronglyTypedFeatureFlag
{
}

public sealed class FeatureOneFeatureFlag : StronglyTypedFeatureFlag, IFeatureOneFeatureFlag
{
    public FeatureOneFeatureFlat(IFeatureManager featureManager)
        : base(featureManager, "FeatureOne")
    {
    }
}

public interface IFeatureTwoFeatureFlag : IStronglyTypedFeatureFlag
{
}

public sealed class FeatureTwoFeatureFlag : StronglyTypedFeatureFlag, IFeatureTwoFeatureFlag
{
    public FeatureTwoFeatureFlat(IFeatureManager featureManager)
        : base(featureManager, "FeatureTwo")
    {
    }
}


public static class FeatureFlagsServiceExtensions
{
    public static IFeatureManagementBuilder AddStronglyTypedFeatureFlags(this IFeatureManagementBuilder builder)
    {
        builder.Services.AddTransient<IFeatureOneFeatureFlag, FeatureOneFeatureFlag>();
        builder.Services.AddTransient<IFeatureTwoFeatureFlag, FeatureTwoFeatureFlag>();
        return builder;
    }
}
