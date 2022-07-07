using BenchmarkDotNet.Running;

namespace TaskRipper.Core.Benchmarks
{
    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<WorkExecutorPerformanceBenchmarks>();
        }
    }
}
