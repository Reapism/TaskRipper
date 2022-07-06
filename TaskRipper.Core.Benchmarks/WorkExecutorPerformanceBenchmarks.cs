using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Loggers;
using System.Runtime.InteropServices;

namespace TaskRipper.Core.Benchmarks
{
    public class WorkExecutorPerformanceBenchmarks
    {
        private static readonly Random Random = Random.Shared;
        private static readonly IWorkExecutor Executor = WorkExecutor.Default;
        private static readonly ILogger Logger = NullLogger.Instance;
        private static readonly ILogger ConsoleLogger = BenchmarkDotNet.Loggers.ConsoleLogger.Default;

        private const int Ten = 10;
        private const int OneHundred = 100;
        private const int OneThousand = 1000;
        private const int TenThousand = 10000;
        private const int OneHundredThousand = 100000;
        private const int OneMillion = 1000000;

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task PrintZerosAndOnesTaskRipper()
        {
            var action = PrintZerosAndOnes();
            var contract = WorkContract.Create(GetExecutionSettings(), action.Method.Name, OneHundredThousand);
            var workAction = new WorkAction(contract, action);
            var unusedToken = new CancellationTokenSource().Token;

            var result = await Executor.ExecuteAsync(workAction, unusedToken);

            //ConsoleLogger.WriteLine($"Result duration for {action.Method.Name} iterating {contract.Iterations} took {result.Duration}");
        }

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task PrintZerosToNTaskRipper()
        {
            var action = PrintZerosToN(Random.Next(OneHundredThousand));
            var contract = WorkContract.Create(GetExecutionSettings(), action.Method.Name, OneHundredThousand);
            var workAction = new WorkAction<int>(contract, action, 0, null, false);
            var unusedToken = new CancellationTokenSource().Token;

            var result = await Executor.ExecuteAsync(workAction, unusedToken);

            //ConsoleLogger.WriteLine($"Result duration for {action.Method.Name} iterating {contract.Iterations} took {result.Duration}");
        }

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task GetZeroAndOnesTaskRipper()
        {
            var func = GetZeroAndOnes();
            var contract = WorkContract.Create(GetExecutionSettings(), func.Method.Name, OneHundredThousand);
            var workFunction = new WorkFunction<int>(contract, func);
            var unusedToken = new CancellationTokenSource().Token;

            var result = await Executor.ExecuteAsync(workFunction, unusedToken);

            //ConsoleLogger.WriteLine($"Result duration for {func.Method.Name} iterating {contract.Iterations} took {result.Duration}");
        }

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task GetZeroToNTaskRipper()
        {
            var func = GetZeroToN(OneHundredThousand);
            var contract = WorkContract.Create(GetExecutionSettings(), func.Method.Name, OneHundredThousand);
            var workFunction = new WorkFunction<int, int>(contract, func, 0, null, false);
            var unusedToken = new CancellationTokenSource().Token;

            var result = await Executor.ExecuteAsync(workFunction, unusedToken);

            //ConsoleLogger.WriteLine($"Result duration for {func.Method.Name} iterating {contract.Iterations} took {result.Duration}");
        }

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task PrintZerosAndOnesDefault()
        {
            var action = PrintZerosAndOnes();

            var startTime = DateTime.Now;
            for (var i = 0; i < OneHundredThousand; i++)
            {
                action();
            }
            var endTime = DateTime.Now;
            var duration = new DateRange(startTime, endTime).Duration;

            //ConsoleLogger.WriteLine($"Result duration for {action.Method.Name} iterating {contract.Iterations} took {duration}");
        }

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task PrintZerosToNDefault()
        {
            var action = PrintZerosToN(Random.Next(OneHundredThousand));

            var startTime = DateTime.Now;
            for (var i = 0; i < OneHundredThousand; i++)
            {
                action(i);
            }
            var endTime = DateTime.Now;
            var duration = new DateRange(startTime, endTime).Duration;

            //ConsoleLogger.WriteLine($"Result duration for {action.Method.Name} iterating {contract.Iterations} took {duration}");
        }

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task GetZeroAndOnesDefault()
        {
            var func = GetZeroAndOnes();

            var startTime = DateTime.Now;
            var queue = new Queue<int>(OneHundredThousand);
            for (var i = 0; i < OneHundredThousand; i++)
            {
                // simulate storing the return of each function call in a queue that won't resize.
                queue.Enqueue(func());
            }
            var endTime = DateTime.Now;
            var duration = new DateRange(startTime, endTime).Duration;

            //ConsoleLogger.WriteLine($"Result duration for {func.Method.Name} iterating {contract.Iterations} took {duration}");
        }

        [Benchmark(OperationsPerInvoke = OneHundredThousand)]
        public async Task GetZeroToNDefault()
        {
            var func = GetZeroToN(OneHundredThousand);

            // Compute duration by invoking a delegate N times in a single threaded loop.
            var startTime = DateTime.Now;
            var queue = new Queue<int>(OneHundredThousand);
            for (var i = 0; i < OneHundredThousand; i++)
            {
                // simulate storing the return of each function call in a queue that won't resize.
                queue.Enqueue(func(i));
            }
            var endTime = DateTime.Now;
            var duration = new DateRange(startTime, endTime).Duration;

            //ConsoleLogger.WriteLine($"Result duration for {func.Method.Name} iterating {contract.Iterations} took {duration}");
        }

        private IExecutionSettings GetExecutionSettings()
        {
            // Ensure the execution range is within the tests execution range.
            return new ExecutionSettings(new Range(1, 8), new Range(1, OneHundredThousand + 1), WorkBalancerOptions.Optimize);
        }

        private Action PrintZerosAndOnes()
        {
            return () => Logger.WriteLine(LogKind.Default, Random.Next(2).ToString());
        }

        private Action<int> PrintZerosToN(int n)
        {
            return (n) => Logger.WriteLine(LogKind.Default, n++.ToString());
        }

        private Func<int> GetZeroAndOnes()
        {
            return () => { return Random.Next(2); };
        }

        private Func<int, int> GetZeroToN(int n)
        {
            return (x) =>
            {
                if (x % 2 == 0)
                    return Random.Next(n++);

                return Random.Next(n+=2);
            };
        }
    }
}
