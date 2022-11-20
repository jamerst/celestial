using CoordinateSharp;

namespace Celestial.Triggers;

public class CelestialTrigger : Trigger
{
    public const string TypeName = "Celestial";

    public override bool IsUtc => true;

    public CelestialTime Time { get; set; }
    public TimeSpan? Offset { get; set; }

    public override DateTime? GetNextOccurrence(DateTime now, Settings settings)
        => GetNextOccurrence(now, now, settings);

    private DateTime? GetNextOccurrence(DateTime now, DateTime day, Settings settings)
    {
        Coordinate c = new Coordinate(
            settings.Latitude ?? throw new ArgumentNullException("Latitude must be provided to use celestial triggers"),
            settings.Longitude ?? throw new ArgumentNullException("Longitude must be provided to use celestial triggers"),
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

            if (result > now)
            {
                return result;
            }
            else
            {
                // return occurrence tomorrow if in past
                return GetNextOccurrence(now, day.AddDays(1), settings);
            }
        }

        return null;
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