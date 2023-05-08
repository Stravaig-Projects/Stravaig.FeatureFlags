using System;
using Microsoft.FeatureManagement;

namespace Stravaig.FeatureFlags;

public static class StronglyTypedFeatureManagementExtensions
{
    public static IFeatureManagementBuilder SetupStronglyTypedFeatures(this IFeatureManagementBuilder builder,
        Action<StronglyTypedFeatureBuilder> options)
    {
        var stronglyTypedBuilder = new StronglyTypedFeatureBuilder(builder);
        options(stronglyTypedBuilder);
        return builder;
    }
}