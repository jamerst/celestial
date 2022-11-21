using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Celestial.Providers;

public class Windows : IProvider
{
    private readonly ILogger _logger;

    public Windows(ILogger<Windows> logger)
    {
        _logger = logger;
    }

    public void SetBackground(string path)
    {
        _logger.LogInformation("Setting Windows desktop background to {path}", path);
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 1, path, SPIF_UPDATEINIFILE);
    }

    public string GetName() => "Windows (SPI)";

    const int SPI_SETDESKWALLPAPER = 0x14;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
}