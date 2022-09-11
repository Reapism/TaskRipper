namespace TaskRipper.Core
{
    public static class RangeExtensions
    {
        /// <summary>
        /// Returns whether the <paramref name="value"/> is in range inclusively to exclusively.
        /// <para>notInRange &lt;= <paramref name="value"/> &lt; notInRange.</para>
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInRange(this Range range, Index value)
        {
            var inStartRange = value.Value >= range.Start.Value;
            var inEndRange = value.Value < range.End.Value;
            return inStartRange && inEndRange;
        }
    }
}
