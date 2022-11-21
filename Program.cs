using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Celestial;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    // hide the command window that is shown when the worker is running
    // ideally this app would run as a Windows Service, but user services aren't a thing on Windows apparently
    WindowsCrap.HideWindow();
}

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddProviders();
    });

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    builder.UseSystemd();
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.UseWindowsService();
}

IHost host = builder.Build();
host.Run();