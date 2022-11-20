namespace Celestial.Providers;

public class Gnome : GSettingsProvider
{
    protected override string Schema => "org.gnome.desktop.background";
    protected override string Key => "picture-uri";

    public override string GetName() => "GNOME (GSettings)";
}