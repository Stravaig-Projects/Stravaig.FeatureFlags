using System.Runtime.CompilerServices;
using DiffEngine;

namespace Stravaig.FeatureFlags.Tests;

public static class ModuleInitialiser
{
    [ModuleInitializer]
    public static void Init()
    {
        Environment.SetEnvironmentVariable("DiffEngine_TargetOnLeft", "true");
        DiffTools.UseOrder(DiffTool.VisualStudioCode, DiffTool.Rider);
        VerifySourceGenerators.Initialize();
    }
}