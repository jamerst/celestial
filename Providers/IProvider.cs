namespace Celestial.Providers;

public interface IProvider
{
    Task SetBackgroundAsync(string path);
    string GetName();
}