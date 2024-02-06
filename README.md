<img src="https://user-images.githubusercontent.com/1627999/168216993-f6dbe7b8-85eb-4863-af1b-bc920f856b55.png" data-canonical-src="https://user-images.githubusercontent.com/1627999/168216993-f6dbe7b8-85eb-4863-af1b-bc920f856b55.png" width="250" height="250" />

# TaskRipper
A library for executing delegates N times, on multiple threads simplifying the balancing of work for each thread, and supporting CancellationTokens.

The delegate supported is `Actionable<TRequest, TResult>` which has a generic input parameter and output return type.

## Purpose
A simple, easy-to-use lightweight, no-dependency library for executing an `Actionable<TRequest, TResult>` delegate N times.
The library provides a way to balance the number of iterations requested by delegating a specific amount of iterations per thread on your machine supporting cancellation tokens per iteration.

## Usage
The intention of the library is ease of use, so this is the minimal code to execute a task. 
Included is creating the `IWorkExecutor` instance which can be injected via constructor.

In two lines essentially, you're able to execute a task.
```csharp
var contract = new WorkContractBuilder()
        .WithIterations(100000)
        .WithCancellationToken(CancellationToken.None) // Optional line, the system will consider empty tokens and disregard them in execution.
        .WithWorkBalancingOptions(WorkBalancerOptions.Optimize)
        .UseDefaultExecutionSettings() // Can provide your own. Most classes have a Default static property for easy access to default objects.
        .Build();

var workResult = await WorkExecutor.Default.ExecuteAsync(contract, ActionableString, request);

private string ActionableString(int request)
{
    return request.ToString();
}
```

Replace the ActionableString function with another one of your choosing to support any return type + any input type.

### What's possible?
* If you want to perform the same action, N times, then the current version and example above will do the trick.

### Limits
* Actions that change their input and outputs between iterations are **not supported** currently. In other words, they are **stateless**, and actions inputs and outputs **do not interact** with one another.

### Future Plan
* Support stateful actions, for example, passing in a Fibonacci number generator, and have it execute N times. Input and Output change
* Fire and Forget support, where you do not have to await the executor and can listen to updates propagated through events.
  
### Contribution
* Ideally, this project opens up interested guests who want to build upon the initial framework and support more action pipelines. For inspiration, check out my YouTube video discussing the future plan and the section above.
