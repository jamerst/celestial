using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Celestial.Providers;
using Celestial.Triggers;

namespace Celestial;
public class Worker : BackgroundService
{
    private readonly IProvider _provider;
    private readonly Settings _settings;
    private readonly ILogger<Worker> _logger;
    private readonly IHostApplicationLifetime _host;

    public Worker(IProvider provider, Settings settings, ILogger<Worker> logger, IHostApplicationLifetime host)
    {
        _provider = provider;
        _settings = settings;
        _logger = logger;
        _host = host;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Triggers.Any())
        {
            _logger.LogCritical("No triggers defined, exiting");
            _host.StopApplication();
            return;
        }

        _logger.LogInformation("Using provider {provider}", _provider.GetName());

        while (!stoppingToken.IsCancellationRequested)
        {
            Trigger? nextTrigger = null;
            DateTime? next = null;

            foreach (var trigger in _settings.Triggers)
            {
                if (!trigger.IsValid(out string? reason))
                {
                    _logger.LogWarning("Invalid trigger: {reason}", reason);
                    continue;
                }

                DateTime? triggerNext;
                try
                {
                    triggerNext = trigger.GetNextOccurrence(trigger.IsUtc ? DateTime.UtcNow : DateTime.Now, _settings);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception thrown when getting next occurrence");
                    continue;
                }

                // convert back to local time if trigger works in UTC
                if (trigger.IsUtc && triggerNext.HasValue)
                {
                    triggerNext = TimeZoneInfo.ConvertTimeFromUtc(triggerNext.Value, TimeZoneInfo.Local);
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
                    await Task.Delay(delay, stoppingToken);
                }

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
            else
            {
                _logger.LogError("No further occurrences found, exiting");
                _host.StopApplication();
                return;
            }
        }
    }
}