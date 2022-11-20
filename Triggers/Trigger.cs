using System.Text.Json;

namespace Celestial.Triggers;

public abstract class Trigger
{
    public string Path { get; set; } = null!;
    public string Type { get; set; } = null!;
    public virtual bool IsUtc => false;

    public abstract DateTime? GetNextOccurrence(DateTime after, Settings settings);

    public bool IsValid(out string? reason)
    {
        if (!File.Exists(Path))
        {
            reason = $@"File ""{Path}"" does not exist";
            return false;
        }

        reason = null;
        return true;
    }
}