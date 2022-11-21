using Microsoft.Extensions.Logging;

namespace Celestial.Providers;

public class Cinnamon : GSettingsProvider
{
    public Cinnamon(ILogger<Cinnamon> logger) : base(logger) {}

    protected override string Schema => "org.cinnamon.desktop.background";
    protected override string Key => "picture-uri";

    public override string GetName() => "Cinnamon (GSettings)";
}