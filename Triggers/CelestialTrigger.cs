using CoordinateSharp;

namespace Celestial.Triggers;

public class CelestialTrigger : Trigger
{
    public const string TypeName = "Celestial";

    public CelestialTime Time { get; set; }
    public TimeSpan? Offset { get; set; }

    public override DateTime? GetPreviousOccurrence(DateTime before, Settings settings)
    {
        DateTime utc = TimeZoneInfo.ConvertTimeToUtc(before, TimeZoneInfo.Local);

        DateTime? result = GetOccurrenceUtc(utc, utc, settings, (r, n) => r <= n, d => d.AddDays(-1));

        if (result.HasValue)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(result.Value, TimeZoneInfo.Local);
        }
        else
        {
            return null;
        }
    }

    public override DateTime? GetNextOccurrence(DateTime now, Settings settings)
    {
        DateTime utc = TimeZoneInfo.ConvertTimeToUtc(now, TimeZoneInfo.Local);

        DateTime? result = GetOccurrenceUtc(utc, utc, settings, (r, n) => r >= n, d => d.AddDays(1));

        if (result.HasValue)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(result.Value, TimeZoneInfo.Local);
        }
        else
        {
            return null;
        }
    }

    private DateTime? GetOccurrenceUtc(DateTime now, DateTime day, Settings settings, Func<DateTime?, DateTime, bool> isInRange, Func<DateTime, DateTime> getNextDay)
    {
        Coordinate c = new Coordinate(
            settings.Latitude ?? throw new InvalidOperationException("Latitude must be provided to use celestial triggers"),
            settings.Longitude ?? throw new InvalidOperationException("Longitude must be provided to use celestial triggers"),
            day
        );

        DateTime? result = Time switch
        {
            CelestialTime.Dawn => c.CelestialInfo.AdditionalSolarTimes.CivilDawn,
            CelestialTime.Sunrise => c.CelestialInfo.SunRise,
            CelestialTime.SunUp => c.CelestialInfo.AdditionalSolarTimes.SunriseBottomDisc,
            CelestialTime.Noon => c.CelestialInfo.SolarNoon,
            CelestialTime.SunSetting => c.CelestialInfo.AdditionalSolarTimes.SunsetBottomDisc,
            CelestialTime.Sunset => c.CelestialInfo.SunSet,
            CelestialTime.Dusk => c.CelestialInfo.AdditionalSolarTimes.CivilDusk,
            _ => null
        };

        if (result.HasValue)
        {
            if (Offset.HasValue)
            {
                result = result.Value + Offset.Value;
            }

            if (!isInRange(result, now))
            {
                return GetOccurrenceUtc(now, getNextDay(day), settings, isInRange, getNextDay);
            }
        }

        return result;
    }

    public override string ToString()
    {
        string prefix;

        if (Offset.HasValue)
        {
            if (Offset > TimeSpan.Zero)
            {
                prefix = $"{Offset} after";
            }
            else
            {
                prefix = $"{Offset.Value.Negate()} before";

            }
        }
        else
        {
            prefix = "at";
        }

        return $"{prefix} {Time}";
    }
}