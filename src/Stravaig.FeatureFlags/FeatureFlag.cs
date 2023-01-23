using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;

namespace Stravaig.FeatureFlags;

public abstract class FeatureFlag : IStronglyTypedFeatureFlag
{
    private static readonly TaskFactory HelperTaskFactory = new(
        CancellationToken.None,
        TaskCreationOptions.None,
        TaskContinuationOptions.None,
        TaskScheduler.Default);

    private bool? _isEnabled = null;
    private readonly IFeatureManager _manager;
    private readonly string _name;

    protected FeatureFlag(IFeatureManager manager, string name)
    {
        _manager = manager;
        _name = name;
    }

    /// <inheritdoc />
    public async Task<bool> IsEnabledAsync()
    {
        _isEnabled ??= await _manager.IsEnabledAsync(_name);
        return _isEnabled.Value;
    }

    /// <inheritdoc />
    public bool IsEnabled()
    {
        if (_isEnabled.HasValue)
            return _isEnabled.Value;

        // This portion of code is based on https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/AsyncHelper.cs
        // which is supplied with an MIT licence.
        // Original: Copyright (c) Microsoft Corporation, Inc. All rights reserved.
        #region AsyncHelper
        var cultureUi = CultureInfo.CurrentUICulture;
        var culture = CultureInfo.CurrentCulture;
        return HelperTaskFactory.StartNew(() =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return IsEnabledAsync();
            })
            .Unwrap()
            .GetAwaiter()
            .GetResult();
        #endregion
    }
}