namespace TaskRipper.Core
{
    public static class RangeExtensions
    {
        /// <summary>
        /// The 
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInRange(this Range range, Index value)
        {
            return value.Value >= range.Start.Value && value.Value < range.End.Value;
        }
    }
}
