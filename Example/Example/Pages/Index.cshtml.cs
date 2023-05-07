using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stravaig.Configuration.Diagnostics.Logging;

namespace Example.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAlphaFeatureFlag _alphaFeature;
    private readonly IBetaFeatureFlag _betaFeature;

    public IndexModel(
        ILogger<IndexModel> logger,
        IAlphaFeatureFlag alphaFeature,
        IBetaFeatureFlag betaFeature,
        IConfiguration config)
    {
        _logger = logger;
        _alphaFeature = alphaFeature;
        _betaFeature = betaFeature;
        _logger.LogConfigurationValues(config.GetSection("FeatureManagement"), LogLevel.Information);

    }

    public void OnGet()
    {
    }

    public bool Alpha => _alphaFeature.IsEnabled();
    public bool Beta => _betaFeature.IsEnabled();

    public string CurrentTime => _betaFeature.IsEnabled()
        ? DateTime.Now.ToString("O")
        : DateTime.Now.ToString("dddd, d MMMM yyyy @ HH:mm:ss zz");
}