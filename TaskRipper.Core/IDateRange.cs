namespace TaskRipper.Core
{
    public interface IDateRange
    {
        DateTime StartDate { get; }

        DateTime EndDate { get; }
    }

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

        public TimeSpan Duration => EndDate - StartDate;
    }
}
