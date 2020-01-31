namespace pdxpartyparrot.Core.Util
{
    public static class UnitUtils
    {
#region Distance
        public static float FeetToMeters(float feet)
        {
            return feet * 0.3f;
        }

        public static float MetersToFeet(float m)
        {
            return m * 3.28f;
        }

        public static float MilesToKilometers(float miles)
        {
            return miles * 1.61f;
        }

        public static float KilometersToMiles(float km)
        {
            return km * 0.62f;
        }
#endregion

#region Speed
        public static float MetersPerSecondToMilesPerHour(float mps)
        {
            return mps * 2.24f;
        }

        public static float MetersPerSecondToKilometersPerHour(float mps)
        {
            return mps * 3.6f;
        }
#endregion

#region Mass / Weight
        public static float PoundsToKilograms(float pounds)
        {
            return pounds * 0.45f;
        }

        public static float KilogramsToPounds(float kg)
        {
            return kg * 2.2f;
        }
#endregion
    }
}
