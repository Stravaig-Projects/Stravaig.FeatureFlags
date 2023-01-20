using System.Threading.Tasks;

namespace Stravaig.FeatureFlags;

public interface IStronglyTypedFeatureFlag
{
    /// <summary>
    /// Gets whether the feature is active or not.
    /// </summary>
    /// <returns>True if the feature is active; false otherwise.</returns>
    Task<bool> IsEnabledAsync();

    /// <summary>
    /// Gets whether the feature is active or not marshalling the result to a synchronous context. This is slow, use the Async variant if possible.
    /// </summary>
    /// <returns>True if the feature is active; false otherwise.</returns>
    bool IsEnabled();
}