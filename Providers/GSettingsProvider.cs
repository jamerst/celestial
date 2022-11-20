namespace Celestial.Providers;

public abstract class GSettingsProvider : IProvider
{
    protected abstract string Schema { get; }
    protected abstract string Key { get; }

    public void SetBackground(string path)
    {
        new GLib.Settings(Schema).SetString(Key, $"file://{path}");
    }

    public abstract string GetName();
}