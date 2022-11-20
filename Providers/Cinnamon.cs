namespace Celestial.Providers;

public class Cinnamon : GSettingsProvider
{
    protected override string Schema => "org.cinnamon.desktop.background";
    protected override string Key => "picture-uri";

    public override string GetName() => "Cinnamon (GSettings)";
}