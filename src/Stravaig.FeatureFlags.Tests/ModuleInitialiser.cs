using System.Runtime.CompilerServices;

namespace Stravaig.FeatureFlags.Tests;

public static class ModuleInitialiser
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}