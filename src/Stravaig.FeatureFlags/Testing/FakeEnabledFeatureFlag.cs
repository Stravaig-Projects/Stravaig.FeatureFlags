using System.Threading.Tasks;

namespace Stravaig.FeatureFlags.Testing;

// This type is used as a base class in generated code.
// ReSharper disable once UnusedType.Global
public abstract class FakeFeatureFlag : IStronglyTypedFeatureFlag
{
    private readonly bool _state;
    protected FakeFeatureFlag(bool state)
    {
        _state = state;
    }

    /// <inheritdoc />
    public Task<bool> IsEnabledAsync() => Task.FromResult(_state);

    /// <inheritdoc />
    public bool IsEnabled() => _state;
}