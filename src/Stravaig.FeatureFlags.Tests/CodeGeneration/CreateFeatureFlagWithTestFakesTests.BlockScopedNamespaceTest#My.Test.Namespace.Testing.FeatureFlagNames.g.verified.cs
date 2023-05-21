﻿//HintName: My.Test.Namespace.Testing.FeatureFlagNames.g.cs
// <auto-generated />
// Namespace: My.Test.Namespace
// Enum: FeatureFlagNames
// IncludeTestFakes: True
// FeatureFlagNames[0]: FeatureOne
// FeatureFlagNames[1]: FeatureTwo

namespace My.Test.Namespace.Testing
{

    public sealed class FakeFeatureOneFeatureFlag : Stravaig.FeatureFlags.Testing.FakeFeatureFlag, IFeatureOneFeatureFlag
    {
        public static readonly FakeFeatureOneFeatureFlag Enabled = new FakeFeatureOneFeatureFlag(true);
        public static readonly FakeFeatureOneFeatureFlag Disabled = new FakeFeatureOneFeatureFlag(false);

        public FakeFeatureOneFeatureFlag(bool state) : base(state)
        {
        }
    }


    public sealed class FakeFeatureTwoFeatureFlag : Stravaig.FeatureFlags.Testing.FakeFeatureFlag, IFeatureTwoFeatureFlag
    {
        public static readonly FakeFeatureTwoFeatureFlag Enabled = new FakeFeatureTwoFeatureFlag(true);
        public static readonly FakeFeatureTwoFeatureFlag Disabled = new FakeFeatureTwoFeatureFlag(false);

        public FakeFeatureTwoFeatureFlag(bool state) : base(state)
        {
        }
    }

}
