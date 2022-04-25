﻿namespace TaskRipper.Core
{
    // TODO
    /// This is an idea to cache certain tasks by method name
    /// and store the best corresponding <see cref="WorkBalancerOptions"/>
    /// to run that task.
    internal class WorkBalancerOptionsCache
    {
        private IDictionary<TaskDescriptor, WorkBalancerOptions> TaskDescriptors { get; }

        public WorkBalancerOptionsCache()
        {
            TaskDescriptors = new Dictionary<TaskDescriptor, WorkBalancerOptions>();
            
            // usage
            //var func = Add;
            //var fullTaskName = func.GetType().FullName;
            //Add(fullTaskName, WorkBalancerOptions.Heavy);
        }

        public WorkBalancerOptions? Get(string fullTaskName)
        {
            var isFound = TaskDescriptors.TryGetValue(new TaskDescriptor(fullTaskName), out var options);
            if (isFound)
                return options;

            return null;
        }

        public bool Add(string fullTaskName, WorkBalancerOptions workBalancerOptions)
        {
            return TaskDescriptors.TryAdd(new TaskDescriptor(fullTaskName), workBalancerOptions);
        }

        public bool Remove(string fullTaskName)
        {
            return TaskDescriptors.Remove(new TaskDescriptor(fullTaskName));
        }
    }

    internal class TaskDescriptor : ITaskDescriptor
    {
        public TaskDescriptor(string fullTaskName)
        {
            FullTaskName = fullTaskName ?? throw new ArgumentNullException(nameof(fullTaskName));
        }

        public string FullTaskName { get; }

        public override int GetHashCode()
        {
            return FullTaskName.GetHashCode() * 23;
        }

        public override bool Equals(object? obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }
    }

    internal interface ITaskDescriptor
    {
        public string FullTaskName { get; }
    }
}