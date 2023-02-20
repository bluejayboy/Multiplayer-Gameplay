namespace Scram
{
    public static class ScramMath
    {
        public static float Mod(float value)
        {
            return ((value %= 360.0f) < 0.0f) ? value + 360.0f : value;
        }

        public static bool IsWithin(int value, int minimumValue, int maximumValue)
        {
            return value >= minimumValue && value <= maximumValue;
        }
    }
}