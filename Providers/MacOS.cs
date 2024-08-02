using CliWrap;
using Microsoft.Extensions.Logging;

namespace Celestial.Providers;

public class MacOS : IProvider
{
    private readonly ILogger _logger;
    public MacOS(ILogger<MacOS> logger)
    {
        _logger = logger;
    }

    public async Task SetBackgroundAsync(string path)
    {
        _logger.LogInformation("Setting macOS desktop background to {path}", path);

        var result = await Cli.Wrap("desktoppr")
            .WithArguments(path)
            .ExecuteAsync();

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to set macOS desktop background. Exit code {code}", result.ExitCode);
        }
    }

    public string GetName() => "macOS";
}