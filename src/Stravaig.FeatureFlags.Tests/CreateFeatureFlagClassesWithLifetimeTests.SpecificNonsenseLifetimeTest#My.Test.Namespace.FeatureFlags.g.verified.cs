﻿//HintName: My.Test.Namespace.FeatureFlags.g.cs
// <auto-generated />

using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Stravaig.FeatureFlags;

namespace My.Test.Namespace;
public interface IAlphaFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class AlphaFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IAlphaFeatureFlag
{
    public AlphaFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Alpha")
    {
    }
}

public interface IBetaFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class BetaFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IBetaFeatureFlag
{
    public BetaFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Beta")
    {
    }
}

public interface IGammaFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class GammaFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IGammaFeatureFlag
{
    public GammaFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Gamma")
    {
    }
}

public interface IDeltaFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class DeltaFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IDeltaFeatureFlag
{
    public DeltaFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Delta")
    {
    }
}

public interface IEpsilonFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class EpsilonFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IEpsilonFeatureFlag
{
    public EpsilonFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Epsilon")
    {
    }
}

public interface IZetaFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class ZetaFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IZetaFeatureFlag
{
    public ZetaFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Zeta")
    {
    }
}

public interface IEtaFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class EtaFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IEtaFeatureFlag
{
    public EtaFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Eta")
    {
    }
}

public interface IThetaFeatureFlag : Stravaig.FeatureFlags.IStronglyTypedFeatureFlag
{
}

public sealed class ThetaFeatureFlag : Stravaig.FeatureFlags.FeatureFlag, IThetaFeatureFlag
{
    public ThetaFeatureFlag(IFeatureManager featureManager)
        : base(featureManager, "Theta")
    {
    }
}


public static class FeatureFlagsServiceExtensions
{
    public static IFeatureManagementBuilder AddFeatureFlags(this IFeatureManagementBuilder builder)
    {
        builder.Services.AddSingleton<IAlphaFeatureFlag, AlphaFeatureFlag>();
#error : (10,14)-(10,30) caused defective source generation. Lifetime of "Garbage" specified is invalid. Using Scoped.
        builder.Services.AddScoped<IBetaFeatureFlag, BetaFeatureFlag>();
#error : (13,14)-(13,30) caused defective source generation. Lifetime of "Rubbish" specified is invalid. Using Scoped.
        builder.Services.AddScoped<IGammaFeatureFlag, GammaFeatureFlag>();
#error : (16,14)-(16,28) caused defective source generation. Lifetime of "Trash" specified is invalid. Using Scoped.
        builder.Services.AddScoped<IDeltaFeatureFlag, DeltaFeatureFlag>();
#error : (19,14)-(19,23) caused defective source generation. Lifetime of "" specified is invalid. Using Scoped.
        builder.Services.AddScoped<IEpsilonFeatureFlag, EpsilonFeatureFlag>();
#error : (22,14)-(22,22) caused defective source generation. Cannot interpret "Lifetime" in this version of the source generator. Please use "Lifetime.<value>".
        builder.Services.AddScoped<IZetaFeatureFlag, ZetaFeatureFlag>();
#error : (25,14)-(25,17) caused defective source generation. Cannot interpret "eta" in this version of the source generator. Please use "Lifetime.<value>".
        builder.Services.AddScoped<IEtaFeatureFlag, EtaFeatureFlag>();
#error : (28,14)-(28,25) caused defective source generation. Cannot interpret "(Lifetime)1" in this version of the source generator. Please use "Lifetime.<value>".
        builder.Services.AddScoped<IThetaFeatureFlag, ThetaFeatureFlag>();
        return builder;
    }
}
