namespace TaskRipper.Core
{
    public interface IDateRange
    {
        DateTime StartDate { get; }

        DateTime EndDate { get; }
    }

    //TODO ensure data is correct by asserting using Guard.
    public class DateRange : IDateRange
    {
        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
        public DateTime StartDate { get; }

        public DateTime EndDate { get; }
    }
}
