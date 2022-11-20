using System.Runtime.InteropServices;

namespace Celestial.Providers;

public class Windows : IProvider
{
    public void SetBackground(string path)
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 1, path, SPIF_UPDATEINIFILE);
    }

    public string GetName() => "Windows (SPI)";

    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 0x01;
    const int SPIF_SENDWININICHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
}