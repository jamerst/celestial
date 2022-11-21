using Microsoft.Extensions.Logging;

namespace Celestial.Providers;

public class Gnome : GSettingsProvider
{
    public Gnome(ILogger<Gnome> logger) : base(logger) {}

    protected override string Schema => "org.gnome.desktop.background";
    protected override string Key => "picture-uri";

    public override string GetName() => "GNOME (GSettings)";
}