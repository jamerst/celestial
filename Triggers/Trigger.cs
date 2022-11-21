namespace Celestial.Triggers;

public abstract class Trigger
{
    public string Path { get; set; } = null!;
    public string Type { get; set; } = null!;

    /// <summary>
    /// Get the previous trigger date/time before <paramref name="now"/>
    /// </summary>
    /// <param name="now">Current date/time</param>
    /// <param name="settings">Settings instance</param>
    /// <returns>The closest trigger date/time before <paramref name="now"/></returns>
    public abstract DateTime? GetPreviousOccurrence(DateTime now, Settings settings);

    /// <summary>
    /// Get the next trigger date/time after <paramref name="now"/>
    /// </summary>
    /// <param name="now">Current date/time</param>
    /// <param name="settings">Settings instance</param>
    /// <returns>The closest trigger date/time after <paramref name="now"/></returns>
    public abstract DateTime? GetNextOccurrence(DateTime now, Settings settings);

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