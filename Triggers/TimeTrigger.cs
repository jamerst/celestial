namespace Celestial.Triggers;

public class TimeTrigger : Trigger
{
    public const string TypeName = "Time";

    public TimeOnly Time { get; set; }

    public override DateTime? GetNextOccurrence(DateTime now, Settings _)
    {
        DateTime today = now.Date + Time.ToTimeSpan();

        if (today > now)
        {
            return today;
        }
        else
        {
            // return occurrence tomorrow if in past
            return today.AddDays(1);
        }
    }

    public override string ToString()
    {
        return $"at {Time}";
    }
}