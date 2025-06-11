public class UnitConverter
{
    public static float ToKilometerPerHour(float meterPerSec) { return meterPerSec * 3.6f; } // value * (60 * 60 / 1000)
    public static float ToMeterPerSec(float kilometerPerHour) { return kilometerPerHour / 3.6f; } // value / (60 * 60 / 1000)
}
