<img src="https://user-images.githubusercontent.com/1627999/168216993-f6dbe7b8-85eb-4863-af1b-bc920f856b55.png" data-canonical-src="https://user-images.githubusercontent.com/1627999/168216993-f6dbe7b8-85eb-4863-af1b-bc920f856b55.png" width="250" height="250"/>

# TaskRipper
A library for executing delegates N times, on multiple threads simplifying the balancing of work for each thread, and supporting CancellationTokens.

## Purpose
A simple, easy to use lightweight, no dependency library for executing `Action`, `Func<T>`, and other parameter variant delegates N times.
The library provides a way to balance the task by delegating a specific amount of iterations per thread.

## Usage
The intention of the library is ease of use, so this is the minimal code to execute a task. 
Included is creating the `IWorkExecutor` instance which can be injected via constructor.

In two lines essentially, you're able to execute a task.
```csharp
var workContract = WorkContract.Create("Say Hello World 100 times", 100);
var workExecutor = WorkExecutor.Default;
var result = await workExecutor.ExecuteAsync(workContract, () => Console.WriteLine("Hello World"), cancellationToken);
```
#### Print 1-1000
```csharp
var workContract = WorkContract.Create("Print 1-1000", 1000);
var workExecutor = WorkExecutor.Default;
var i = 1;
var result = await workExecutor.ExecuteAsync(workContract, (i) => Console.WriteLine(i.ToString()), cancellationToken);
```

