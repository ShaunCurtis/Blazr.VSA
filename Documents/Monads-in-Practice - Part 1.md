# C# Monads in Practice - Part 1 - Our First Monad

Let's start with some basic FP principles we need to adhere to.  I'll cover them in more detailas we write code.

1. Data is immutable.
1. Functions are values.
1. Everything's an expression.
1. Functions are pure.

### What is a Monad?

Forget understanding the maths and category theory: its just a coding pattern.  It's a wrapper object for other objects that provides some specific functionality.  Think `<IEnumerabe>T` with *Linq*.

The functionality is:

1. Methods to wrap `T`: constructors.
1. Methods to chain operations on T to the Monad.
1. One or methods to output T.

### The Demo App

Consider:

```csharp
var input = Console.ReadLine();
Console.WriteLine(input);
```

You could write this in a more functionally like this:

```csharp
Console.WriteLine(
    Console.ReadLine()
);
```

### Our first Monad.

What we're doing above is I/0.  Input from the console and outputting to the console.

we can declare `IOMonad` like this:

```csharp
public readonly record struct IOMonad<T>(T Value);
```

Points:
1. FP - It's immutable
2. Monad - the `new` constructor provides a method to wrap `T`

### Wrapping/Constructors

While `new` works, static constructors provide an FP approach.  These are declared in a separate static non-generic class:

```csharp
public static class IOMonad
{
    public static IOMonad<T> Input<T>(T value)
        => new IOMonad<T>(value);

    public static IOMonad<T> Input<T>(Func<T> input)
        => new IOMonad<T>(input.Invoke());
}
```

Note the second one takes a method with the pattern `Func<T>`. We can use it to get input directly from `Console.ReadLine`. 

```csharp
IOMonad.Input(Console.ReadLine())
```

### Output

We can output the value directly:

```csharp
public T Output()
    => this.Value;
```

or use an `Action` delegate:

```csharp
public void Output(Action<T> output)
    => output.Invoke(this.Value);
```

So we can pass `Console.WriteLine` to `Output`.  

```csharp
IOMonad
    // other operations
    .Output(Console.WriteLine);
```

Points:
1. Functions are values - passed a arguments into a method.
2. Operations are chained.

### Chaining

Chaining or applying sequential operations is achieved using the *Map* and *Bind* patterns. 

#### Bind Pattern

`Bind` accepts any method with the pattern `Func<T, IOMonad<TOut>>`.

```csharp
public IOMonad<TOut> Bind<TOut>(Func<T, IOMonad<TOut>> func)
    => func.Invoke(this.Value);
```

Here's an example using a lambda expression:

```csharp
IOMonad.Input(Console.ReadLine())
    .Bind( value => IOMonad.Input(value?.ToUpper() ?? "[Null]"))
    .Output(Console.WriteLine);
```

And can be chained:

```csharp
IOMonad.Input(Console.ReadLine())
    .Bind( value => IOMonad.Input(value?.ToUpper() ?? "[Null]"))
    .Bind(value => IOMonad.Input($"The Value is:{value}"))
    .Output(Console.WriteLine);
```

The *Bind* pattern is known as a *Monadic Function*.

#### Map Pattern

`Map` accepts any method with the pattern `Func<T, TOut>`.

```csharp
public IOMonad<TOut> Map<TOut>(Func<T, TOut> func)
    =>IOMonad.Input(func.Invoke(this.Value));
```

And can be used like this:

```csharp
IOMonad.Input(Console.ReadLine())
    .Map(value => value?.Trim() ?? null)
    .Bind( value => IOMonad.Input(value?.ToUpper() ?? "[Null]"))
    .Bind(value => IOMonad.Input($"The Value is:{value}"))
    .Output(Console.WriteLine);
```

Any object implementing the *Map* pattern is a **Functor**.

## Appendix

The full code:

```csharp
public readonly record struct IOMonad<T>(T Value)
{
    public T Output()
        => this.Value;

    public void Output(Action<T> output)
        => output.Invoke(this.Value);

    public IOMonad<TOut> Bind<TOut>(Func<T, IOMonad<TOut>> func)
        => func.Invoke(this.Value);

    public IOMonad<TOut> Map<TOut>(Func<T, TOut> func)
        =>IOMonad.Input(func.Invoke(this.Value));
}
```

```csharp
public static class IOMonad
{
    public static IOMonad<T> Input<T>(Func<T> input)
        => new IOMonad<T>(input.Invoke());

    public static IOMonad<T> Input<T>(T value)
        => new IOMonad<T>(value);
}
```