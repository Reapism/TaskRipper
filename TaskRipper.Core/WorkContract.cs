using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRipper.Core
{

    public class WorkContract : IWorkContract
    {
        public WorkContract(IExecutionSettings executionSettings, string description, int iterations = 1)
        {
            ExecutionSettings = executionSettings ?? throw new ArgumentNullException(nameof(executionSettings));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Iterations = iterations > 0 ? iterations : 1;
        }

        public string Description { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public int Iterations { get; }

        public IExecutionSettings ExecutionSettings { get; }
    }
}
