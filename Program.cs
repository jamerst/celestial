using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Celestial;

Settings settings = await Settings.LoadFromFileAsync();

var cultureInfo = CultureInfo.CurrentCulture;

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddProviders();
        services.AddSingleton(settings);
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