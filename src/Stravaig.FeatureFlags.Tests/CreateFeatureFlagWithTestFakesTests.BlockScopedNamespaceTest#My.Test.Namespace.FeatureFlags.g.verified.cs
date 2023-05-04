﻿//HintName: My.Test.Namespace.FeatureFlags.g.cs
// <auto-generated />

using Microsoft.FeatureManagement;
using Stravaig.FeatureFlags;

namespace My.Test.Namespace
{
    public interface IFeatureOneFeatureFlag : IStronglyTypedFeatureFlag
    {
    }

    public sealed class FeatureOneFeatureFlag : FeatureFlag, IFeatureOneFeatureFlag
    {
        public FeatureOneFeatureFlag(IFeatureManager featureManager)
            : base(featureManager, "FeatureOne")
        {
        }
    }

    public interface IFeatureTwoFeatureFlag : IStronglyTypedFeatureFlag
    {
    }

    public sealed class FeatureTwoFeatureFlag : FeatureFlag, IFeatureTwoFeatureFlag
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
            builder.Services.AddScoped<IFeatureOneFeatureFlag, FeatureOneFeatureFlag>();
            builder.Services.AddScoped<IFeatureTwoFeatureFlag, FeatureTwoFeatureFlag>();
            return builder;
        }
    }
}
