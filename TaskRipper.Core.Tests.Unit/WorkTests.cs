using System;
using System.Threading;
using Xunit.Abstractions;

namespace TaskRipper.Core.Tests.Unit
{
    public class WorkTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public WorkTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        private Action WriteHelloWorld => () =>
        {
            testOutputHelper.WriteLine("Hello World");
        };

        private Action<int> WriteI => (i) =>
        {
            testOutputHelper.WriteLine(i.ToString());
        };

        private Action<int, int> WriteIPlusJ => (i, j) =>
        {
            testOutputHelper.WriteLine((i + j).ToString("n0"));
        };

        private Action<int> WriteIMSM => (i) =>
        {
            if (i % 2 == 0)
            {
                i++;
            }
        };

        private Action<int, int> WriteIPlusJMSM => (i, j) =>
        {
            if (i % 2 == 0)
            {
                i++;
            }
            if (j % 2 == 0)
            {
                j *= 2;
            }
        };
    }
}
