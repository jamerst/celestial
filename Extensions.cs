using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

using Celestial.Providers;

namespace Celestial;

public static class Extensions
{
    public static IServiceCollection AddProviders(this IServiceCollection services)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string? currentDesktop = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
            if (currentDesktop != null)
            {
                if (currentDesktop.Contains("GNOME", StringComparison.OrdinalIgnoreCase))
                {
                    services.AddSingleton<IProvider, Gnome>();
                }
                else if (currentDesktop.Contains("Cinnamon", StringComparison.OrdinalIgnoreCase))
                {
                    services.AddSingleton<IProvider, Cinnamon>();
                }
                else
                {
                    throw new InvalidOperationException($@"Unsupported desktop environment ""{currentDesktop}""");
                }
            }
            else
            {
                throw new InvalidOperationException("Unable to determine desktop environment, XDG_CURRENT_DESKTOP variable is not set");
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSingleton<IProvider, Windows>();
        }

        return services;
    }
}