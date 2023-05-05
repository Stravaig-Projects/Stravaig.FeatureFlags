using System.Threading.Tasks;

namespace Stravaig.FeatureFlags.Testing;

public class FakeFeatureFlag : IStronglyTypedFeatureFlag
{
    private readonly bool _state;
    public FakeFeatureFlag(bool state)
    {
        _state = state;
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(_state);

    public bool IsEnabled() => _state;
}