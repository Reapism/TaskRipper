namespace TaskRipper.Core
{
    public static class RangeExtensions
    {
        public static bool IsInRange(this Range range, Index value)
        {
            return value.Value >= range.Start.Value && value.Value < range.End.Value;
        }
    }
}
