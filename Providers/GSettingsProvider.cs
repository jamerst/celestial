using Microsoft.Extensions.Logging;

namespace Celestial.Providers;

public abstract class GSettingsProvider : IProvider
{
    private readonly ILogger _logger;

    public GSettingsProvider(ILogger<GSettingsProvider> logger)
    {
        _logger = logger;
    }

    protected abstract string Schema { get; }
    protected abstract string Key { get; }

    public void SetBackground(string path)
    {
        _logger.LogInformation("Setting GSettings desktop background to {path}", path);

        new GLib.Settings(Schema).SetString(Key, $"file://{path}");
    }

    public abstract string GetName();
}