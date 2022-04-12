namespace TaskRipper.Core
{
    public interface IDateRange
    {
        DateTime StartDate { get; }

        DateTime EndDate { get; }
    }

    // TODO ensure data is correct by asserting using Guard.
    // TODO finish the stubs
    public class DateRange : IDateRange
    {
        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                throw new DateOutOfRangeException(startDate, endDate);
            }

            StartDate = startDate;
            EndDate = endDate;
        }
        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public TimeSpan Duration { get { return EndDate - StartDate; } }
    }
}
