namespace Stravaig.FeatureFlags;

public enum Lifetime
{
    /// <summary>
    /// The flag will be injected as a transient object.
    /// Each getting the most up-to-date value of the flag.
    /// </summary>
    Transient,

    /// <summary>
    /// The flag will be injected as a scoped object.
    /// The value of the flag will remain the same for that scope even if it changes externally.
    /// </summary>
    Scoped,

    /// <summary>
    /// The flag will be injected as a singleton object.
    /// The value of the flag will remain the same for the lifetime of the DI container,
    /// which is typically the lifetime of the process.
    /// </summary>
    Singleton,
}