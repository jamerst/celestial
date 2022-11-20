namespace Celestial.Providers;

public interface IProvider
{
    void SetBackground(string path);
    string GetName();
}