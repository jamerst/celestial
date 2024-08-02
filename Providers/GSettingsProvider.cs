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

    public Task SetBackgroundAsync(string path)
    {
        _logger.LogInformation("Setting GSettings desktop background to {path}", path);

        new GLib.Settings(Schema).SetString(Key, $"file://{path}");

        return Task.CompletedTask;
    }

    public abstract string GetName();
}