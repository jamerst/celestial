using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Celestial.Providers;
using Celestial.Triggers;

namespace Celestial;
public class Worker : BackgroundService
{
    private readonly IProvider _provider;
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _host;

    private Settings settings = null!;
    private CancellationTokenSource ctsConfig = null!;
    private CancellationTokenSource ctsCombined = null!;

    public Worker(IProvider provider, ILogger<Worker> logger, IHostApplicationLifetime host)
    {
        _provider = provider;
        _logger = logger;
        _host = host;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadSettingsAsync(stoppingToken);

        if (!settings.Triggers.Any())
        {
            _logger.LogCritical("No triggers defined, exiting");
            _host.StopApplication();
            return;
        }

        _logger.LogInformation("Using provider {provider}", _provider.GetName());

        using (var watcher = new FileSystemWatcher(Path.GetDirectoryName(GetConfigPath())!))
        {
            watcher.Filter = ConfigFileName;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            watcher.Changed += OnConfigFileChange;

            SetInitialBackground();

            await RunAsync(stoppingToken);
        }
    }

    private void SetInitialBackground()
    {
        var previousTrigger = settings.Triggers
            .Select(t => new { Trigger = t, Today = t.GetPreviousOccurrence(DateTime.Today, settings) })
            .Where(t => t.Today < DateTime.Now)
            .OrderByDescending(t => t.Today)
            .FirstOrDefault();

        if (previousTrigger != null)
        {
            _logger.LogInformation("Setting initial state from previous trigger {trigger} ({time})", previousTrigger.Trigger, previousTrigger.Today?.ToString("s"));
            try
            {
                _provider.SetBackground(previousTrigger.Trigger.Path);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown when setting background");
            }
        }
        else
        {
            _logger.LogWarning("Could not determine initial state from previous trigger");
        }
    }

    private async Task RunAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Trigger? nextTrigger = null;
            DateTime? next = null;

            foreach (var trigger in settings.Triggers)
            {
                if (!trigger.IsValid(out string? reason))
                {
                    _logger.LogWarning("Invalid trigger: {reason}", reason);
                    continue;
                }

                DateTime? triggerNext;
                try
                {
                    triggerNext = trigger.GetNextOccurrence(DateTime.Now, settings);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception thrown when getting next occurrence");
                    continue;
                }

                if (triggerNext < next || (!next.HasValue && triggerNext.HasValue))
                {
                    nextTrigger = trigger;
                    next = triggerNext;
                }
            }

            if (next.HasValue && nextTrigger != null && next > DateTime.Now)
            {
                // calculate time until next trigger fires
                TimeSpan delay = next.Value - DateTime.Now;
                if (delay > TimeSpan.Zero)
                {
                    _logger.LogInformation("Next trigger is {trigger} in {delay} ({time})", nextTrigger, delay, next?.ToString("s"));

                    // wait until trigger time (if in future)

                    try
                    {
                        await Task.Delay(delay, ctsCombined.Token);
                    }
                    catch (TaskCanceledException e)
                    {
                        if (e.CancellationToken == ctsCombined.Token)
                        {
                            if (ctsConfig.IsCancellationRequested && !stoppingToken.IsCancellationRequested)
                            {
                                _logger.LogInformation("Config file change detected, reloading settings");
                                await LoadSettingsAsync(stoppingToken);
                                SetInitialBackground();
                                continue;
                            }
                        }
                        else
                        {
                            throw e;
                        }
                    }
                }

                if (!stoppingToken.IsCancellationRequested)
                {
                    // set background once trigger time is reached
                    _logger.LogInformation("Changing background to {background}", nextTrigger.Path);

                    try
                    {
                        _provider.SetBackground(nextTrigger.Path);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Exception thrown when setting background");
                    }
                }
            }
            else
            {
                _logger.LogError("No further occurrences found, exiting");
                _host.StopApplication();
                return;
            }
        }
    }

    private void OnConfigFileChange(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
        {
            ctsConfig.Cancel();
        }
    }

    private async Task LoadSettingsAsync(CancellationToken stoppingToken)
    {
        settings = await Settings.LoadFromFileAsync(GetConfigPath(), _logger);
        ctsConfig = new CancellationTokenSource();
        ctsCombined = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, ctsConfig.Token);
    }

    private string GetConfigPath() => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "celestial",
            ConfigFileName
        );

    private const string ConfigFileName = "config.json";
}