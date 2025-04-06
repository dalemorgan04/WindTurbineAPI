using WindTurbineApi.Domain.Enums;

namespace WindTurbineApi.Domain.ValueObjects
{
    public record ReadingValue
    {
        public double Value { get; init; }
        public ReadingUnit Unit { get; init; }

        public ReadingValue(double value, ReadingUnit unit)
        {
            Value = value;
            Unit = unit;
        }
    }
}
