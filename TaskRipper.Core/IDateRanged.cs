namespace TaskRipper.Core
{
    public interface IDateRanged
    {
        DateTime StartDate { get; }

        DateTime EndDate { get; }
    }
}
