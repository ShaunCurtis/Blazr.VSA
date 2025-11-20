# C# Monads in Practice - Part 2 - Our Second Monad

In Part 1 we learned about the *Monad* pattern how to build a simple I/O monad.  In this article we'll develop a more complex nomad to handle:

1. Input Errors/Exceptions.
2. Nullable returns : `T? GetData(TId id)`

and provide a path for errors/excptions to flow from inputto output.

## Our Demo

```csharp
var input = Console.ReadLine();
try
{
    var result = double.Parse(input!);
    result = Math.Sqrt(result);
    result = Math.Round(result, 2);
    Console.WriteLine($"The result is {result}");
}
catch
{
    Console.WriteLine("The input value could not be parsed.");
}
```

I'm not using `TryParse` because I want to demonstrate catching exceptions 

## Bool<T>


