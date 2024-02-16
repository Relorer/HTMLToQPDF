using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Utils
{
    internal static class UnitUtils
    {
        public static float ToPoints(float value, Unit unit)
        {
            return value * GetConversionFactor();
            float GetConversionFactor()
            {
                return unit switch
                {
                    Unit.Point => 1f,
                    Unit.Meter => 2834.64575f,
                    Unit.Centimetre => 28.3464565f,
                    Unit.Millimetre => 2.83464575f,
                    Unit.Feet => 864f,
                    Unit.Inch => 72f,
                    Unit.Mil => 0.072f,
                    _ => throw new ArgumentOutOfRangeException("unit", unit, null),
                };
            }
        }
    }
}