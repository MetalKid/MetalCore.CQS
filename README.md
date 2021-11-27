# MetalCore.CQS

## What is CQS?

CQS stands for Command/Query Separation. In its simplest definition, this just means all your queries 
(Get/Read) are defined/stored separately from all your commands (Create, Update, Delete). 
The queries are specialized to return the exact data you need to return, 
while the commands can take some sort of input and persist with whatever underlying structure you want 
(like DDD or straight entities in EF).

### Query
A query simply means when you run the code, the answer doesn’t change. If you call a query over and over, 
you should get the same answer each time, assuming the data being read behind it stays the same. 

### Command
A command means you will be changing the state every time you run the code. It also cannot return any data 
from the command that just finished.

### CommandQuery
However, like most things in life, there are gray areas. In some cases, such as queues/stacks, you must change 
the state in order to read the value. In other cases, like auto-incrementing ids, the id isn’t assigned until 
the create happens, so it becomes troublesome if you need that primary key back to redirect the user to a detail page, 
for example.

Many people try to solve this problem in different ways. Some people try to go out of their way to keep pure CQS with events, 
Guids in another column used to read the data out later, or other more complicated ways. Instead, MetalCore.CQS
embraces those one-off situations by creating “CommandQueries”. They are essentially commands that return data 
that you can’t go forward without. By adding CommandQueries, all the situations that can occur will be covered.

## CQS Interfaces

When implementing CQS, you will need to use interfaces to implement the style and get the plumbing hooked up. 
These include IQuery, IQueryHandler, ICommand, and ICommandHandler. In this version, 
Metal.CQS forces async/await through the chain and forces an IResult type back in order to avoid throwing exceptions 
everywhere. You will tend to see these interfaces defined something like this:

```csharp
public interface IQuery<TResult>
{   
}
```

```csharp
public interface IQueryHandler<in TQuery, TResult> where TQuery: IQuery<TResult>
{   
	Task<IResult<TResult>> ExecuteAsync(TQuery query, CancellationToken token = default);
}   
```

In this case, the TQuery is the request (filter) from the client/user, and the TResult is what will be returned when completed. 
IResult<TResult> will have a Data property returned with its generic type.

```csharp
public interface ICommand
{
}
```

```csharp
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
        Task<IResult> ExecuteAsync(TCommand command, CancellationToken token = default);
}
```

For commands, you just have the TCommand – which is the data being used to change the state. 
The IResult being returned describes if it was successful or what went wrong, so returning this 
does not break CQS.  You can throw exceptions instead and ignore the IResult if you want.

If you also want to go the ICommandQuery route, those would look something like this:

```csharp
public interface ICommandQuery<TResult>
{
}
```

```csharp
public interface ICommandQueryHandler<in TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
{
        Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery, CancellationToken token = default);
}
```

This essentially combines both Command and Query styles into one spot. Using a CommandQuery 
should always be a last resort in order to keep CQS as pure as possible. Only use this when
you need to return something from a command running that you can't get from a query later.

## Usefulness

So, what exactly can we accomplish with these interfaces and why does this help anything? 
Let’s take a website, for example. When a user goes to a website, they will perform a lot of 
GETs/Queries and a few Writes/Commands. Each point of interaction with a user can 
become its own query/command that is coded with a specific goal in mind. You end up always 
interacting with a controller if you use ASP.NET MVC. Each method on your controller is either 
going to be a Query, Command, or CommandQuery and can be mapped 1:1 with a corresponding handler. 
Each method will either take in an ICommand, IQuery, or ICommandQuery based input (or create one inside the method) 
that will then be used to call the correct handler.

However, this means that all these handlers for these queries/commands end up being their own class. 
Each method on a controller will end up calling an individual class that stores the code. 
You end up with an explosion of classes. Some people will feel this is a bad thing, 
but the pros far outweigh the cons. The easiest thing to do to mitigate this is to have a 
well-defined folder structure in place so you can easily find your classes (features) and 
know where to add them. Since each class is used to complete the single task, you know all 
the dependencies of the class are needed to complete the request. This pretty much forces you to 
follow SOLID, which also results in a unit test being very specific to what you are working on 
and much easier to write. Finally, many developers can be working on them all at once and 
not have to worry about merge conflicts. However, the real power of this setup is using Decorators.

## Mediators

Since each query/command is 1:1 with its handler, you can rely on something called a mediator to 
automatically dispatch that request to the correct handler. The long handler interface names become 
tedious to define with IoC injection at the controller level, so you can use a mediator instead. 
A mediator will take the Query/Command/CommandQuery and look up the corresponding handler type for you 
and call it. This makes the controller code itself not only small but look the same between them. 
You can add your own custom logic in a mediator to do any global pre- or post-processing if you need to 
because all the controllers end up calling this same shared class.  You can use the built in mediators
this has to offer or create your own.  The mediator that comes with MetalCore.CQS is done a little
bit differently.  Instead of passing the container in, it takes an anonymous callback function so
it doesn't have a direct reference to the IoC controller.  This makes it trivial to change IoC 
providers in the future and also greatly reduces the code needed to make this work.

## Decorators
The simplest definition of a decorator is something that wraps a class and adds functionality without 
changing the target class. In the case of CQS, we can create additional functionality by having a 
decorator’s constructor take in the same interface it is implementing, run its logic, and then call 
the next decorator/handler in the chain. You can add cross-cutting concerns in a single class that 
affects all handlers that exist now or in the future. You can add things like exception handling, 
logging, retry, caching, timings, permissions, validation, cache invalidation and more in a single 
class that can be unit tested and allow the handlers to fully follow SOLID. 

Several decorators come with this CQS implemenation if you wish to use them.  Some 
require specific verbiage (e.g. translations), so you need to implement some of it yourself as they
are abstract classes.

Please see the Sample code for examples of how everything is hooked up and used.