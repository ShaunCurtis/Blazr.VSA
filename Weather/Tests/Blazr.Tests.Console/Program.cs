//using Blazr.Manganese;
using System;
using System.Data.SqlTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Console.WriteLine(
//    Result<string>
//        .ExecuteFunction<string>(Console.ReadLine)
//        .OutputValue<string>(
//            hasValue: (value) => $"Success: The transformed value is: {value}",
//            hasException: (ex) => $"Failure: {ex.Message}"
//        )
//    );

//Console
//    .ReadLine()
//    .ParseForInt()
//    .ExecuteFunction((value) => Math.Sqrt(value))
//    .ExecuteFunction((value) => Math.Round(value, 2))
//    //  Handle the functions
//    .Output(
//        hasValue: (value) => Console.WriteLine($"Parsed successfully: The functioned value is: {value}"),
//        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
//    );

//... functions



new Result<string?>(Console.ReadLine())
    .ExecuteFunction<double>(double.Parse);

//if(double.TryParse(input, out double value))
//{
//    value = Math.Sqrt(value);
//    Console.WriteLine($"The square root is: {Math.Round(value, 2)}");
//}
//else
//{
//    Console.WriteLine($"The input is not a valid");
//}


public record Result<T>
{
    public T? Value { get; private init; }
    public Exception? Exception { get; private init; }

    public Result(T value) : this(value, null) { }

    public Result(Exception exception) : this(default, exception) { }

    private Result(T? value, Exception? exception)
    {
        Value = value;
        Exception = exception;
    }

    public Result<TOut> ExecuteFunction<TOut>(Func<T, Result<TOut>> function)
        => this.Exception is null
            ? function(Value!)
            : new Result<TOut>(this.Exception);

    public Result<TOut> ExecuteFunction<TOut>(Func<T, TOut> function)
    {
        if (this.Exception is not null)
            return new Result<TOut>(this.Exception!);

        try
        {
            var value = function.Invoke(this.Value!);
            return (value is null)
                ? new Result<TOut>(new ResultException("The function returned a null value."))
                : new Result<TOut>(value);
        }
        catch (Exception ex)
        {
            return new Result<TOut>(ex);
        }
    }

    public T OutputValue(Func<Exception, T> hasException)
    {
        if (this.Exception is not null)
            return hasException.Invoke(Exception!);

        return this.Value!;
    }
}
//public Result<TOut> ExecuteResult<TOut>(Func<T, TOut> function)
//{
//    if (HasValue)
//    {
//        try
//        {
//            return Result<TOut>.Create(mapFunction(Value!));
//        }
//        catch (Exception ex)
//        {
//            return Result<TOut>.Failure(new ResultException(ex.Message));
//        }
//    }
//    else
//    {
//        return Result<TOut>.Failure(Exception!);
//    }
//}


public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }
}

//if (input is null)
//     Console.WriteLine($"The input was null");
//else if(!input.All(char.IsNumber))
//    Console.WriteLine($"The input isnot a numberl");
//else
//{


//}



//if (int.TryParse(input, out int value))
//{
//    parseResult = Result<int>.Create(value);
//}
//else
//{
//    parseResult = Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
//}

//var parseResult = int.TryParse(input, out int value)
//    ? Result<int>.Create(value)
//    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));

//if (parseResult.HasValue)
//{
//    double functionedValue = Math.Sqrt(parseResult.Value);
//    functionedValue = Math.Round(functionedValue, 2);
//    Console.WriteLine($"Parsed successfully: The functioned value is: {functionedValue}");
//}
//else
//{
//    Console.WriteLine(parseResult.Exception!.Message);
//}

//Result<int> ParseForInt(string? input) =>
//    int.TryParse(input, out int value)
//    ? Result<int>.Create(value)
//    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));


//double doubleValue = 0;

//await Console
//    .ReadLine()
//    .ToResult()
//    .ExecuteFunctionAsync(Utils.StringToDoubleAsync)
//    .MutateStateAsync((value) => doubleValue = value)
//    .ExecuteFunctionAsync(Math.Sqrt)
//    .ExecuteFunctionAsync((value) => Math.Round(value, 2))
//    .OutputAsync(
//        hasValue: (value) => Console.WriteLine($"Parsed successfully: The functioned value of {doubleValue} is: {value}"),
//        hasException: (ex) => Console.WriteLine($"Failed: {ex.Message}")
//    );

//if (result.HasValue)
//{
//    //...
//    Console.WriteLine($"Parsed successfully: The functioned value is: {result.Value}");
//}
//else
//{
//    Console.WriteLine($"Failed to parse input: {result.Exception!.Message}");
//}

//Result<int> ParseForInt(string input)
//{
//    if (!string.IsNullOrEmpty(input))
//    {
//        try
//        {
//            var value = int.Parse(input!);

//            var result = Math.Sqrt(value);
//            result = Math.Round(result, 2);
//            return Result<int>.Create((int)result);
//        }
//        catch (Exception ex)
//        {
//            return Result<int>.Failure(ex);
//        }
//    }
//    else
//    {
//        return Result<int>.Failure("Input cannot be null or empty.");
//    }
//}

//if (!string.IsNullOrEmpty(input))
//{
//    try
//    {
//        var value = int.Parse(input!);

//        var result = Math.Sqrt(value);
//        result = Math.Round(result, 2);
//        Console.WriteLine($"Parsed successfully: The functioned value of {value} is: {result}");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"An exception occurred: {ex.Message}");
//    }
//}
//else
//{
//    Console.WriteLine("Input cannot be null or empty.");
//}



//Result<string>.Create(value)
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//Result<string>.Create(value)
//    .Output(
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//value = null;

//Result<string>.Create(value)
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//Result<string>.Create(value)
//    .Map((v) => Result<string>.Create(v.ToUpper()))
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//Result<string>.Create(value)
//    .Map(ToUpper)
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );


public static class Utils
{
    public static Func<string?, Task<Result<double>>> StringToDoubleAsync = async (input)
        =>
    {
        await Task.Yield();
        return double.TryParse(input, out double value)
        ? Result<double>.Create(value)
        : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    };

    public static async Task<Result<double>> ParseForDoubleAsync(this string? input)
    {
        await Task.Yield();
        return double.TryParse(input, out double value)
        ? Result<double>.Create(value)
        : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    }
}


public static class stringExtensions
{
    public static Result<string> ToResult(this string? input) =>
        Result<string>.Create(input);

    public static async Task<Result<double>> ParseForDouble(this string? input)
    {
        await Task.Yield();
        return double.TryParse(input, out double value)
        ? Result<double>.Create(value)
        : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    }

    public static Result<int> ParseForInt(this string? input) =>
        int.TryParse(input, out int value)
        ? Result<int>.Create(value)
        : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    //    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
}


//var xresult = Result<string>.Create(input)
//  .ExecuteFunction<string>(ToUpper)
//  .ToResult;

//DisplayError(xresult);



//Result<string> ToUpper(string value)
//     => string.IsNullOrEmpty(value)
//         ? Result<string>.Failure("Value cannot be null or empty")
//         : Result<string>.Create(value.ToUpper());

//void DisplayError(Result result)
//{
//    result.Output(
//        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );
//}

