using System.Threading.Tasks;

namespace Stravaig.FeatureFlags.Testing;

public class FakeEnabledFeatureFlag : IStronglyTypedFeatureFlag
{
    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public bool IsEnabled() => true;
}