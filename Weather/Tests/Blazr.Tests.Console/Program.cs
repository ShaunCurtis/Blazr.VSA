﻿//using Blazr.Manganese;
using System;
using System.Data.SqlTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

//var value = Console.ReadLine();

//if (value is null)
//    Console.WriteLine("Value is Null.");
//else
//{
//    try
//    {
//        var output = double.Parse(value!);
//        Console.WriteLine($"Value is {output}.");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Value was not a number. Error: {ex.Message}.");
//    }
//}

Console.WriteLine(
    Console.ReadLine()
    .ToResult()
    .MapFunction(Double.Parse)
    .MapFunction(Math.Sqrt)
    .BindFunction((value) => Result<double>.Create(Math.Round(value, 2)))
    .OutputValue<string>(
        hasValue: (value) => $"Success: The transformed value is: {value}",
        hasException: (ex) => $"Failure: {ex.Message}"
    ));

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


//Console.ReadLine()
//   .ToResult()
//   .ExecuteFunction<double>(double.Parse)
//   .Output(
//        hasValue: (value) => Console.WriteLine($"Value is: {value}"),
//        hasException: (exception) => Console.WriteLine($"Error: {exception.Message}")
//    );

//Console.WriteLine(
//    Console.ReadLine()
//   .ToResult()
//   .ExecuteFunction<double>(double.Parse)
//   .ExecuteFunction(Math.Sqrt)
//   .ExecuteFunction((value) => Math.Round(value, 2))
//   .OutputValue<string>(
//        hasValue: (value) => $"Value is: {value}",
//        hasException: (exception) => $"Error: {exception.Message}"
//    ));

public static class ResultExtensions
{
    public static Result<string> ToResult(this string? value)
        => value is null
            ? new Result<string>(ResultException.Create("Value can'tbe null."))
            : new Result<string>(value);
}

public record Result<T>
{
    private T? Value { get; init; }
    private Exception? Exception { get; init; }

    private Result(T? value, Exception? exception)
    {
        Value = value;
        Exception = exception;
    }

    public Result(T value) : this(value, null) { }

    public Result(Exception exception) : this(default, exception) { }

    public static Result<T> Create(T? value)
        => value is null
            ? new(default, ResultException.Create("Value was null"))
            : new(value);

    public static Result<T> Error(string message)
        => new(default, ResultException.Create(message));

    public Result<TOut> BindFunction<TOut>(Func<T, Result<TOut>> function)
        => this.Exception is null
            ? function(Value!)
            : new Result<TOut>(this.Exception);

    public Result<TOut> MapFunction<TOut>(Func<T, TOut> function)
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

    public Result<T> Output(Action<T> hasValue, Action<Exception> hasException)
    {
        if (this.Exception is not null)
            hasException.Invoke(Exception!);
        else
            hasValue.Invoke(this.Value!);

        return this;
    }

    public T OutputValue(Func<Exception, T> hasException)
        => this.Exception is null
            ? this.Value!
            : hasException.Invoke(Exception!);

    public TOut OutputValue<TOut>(Func<T, TOut> hasValue, Func<Exception, TOut> hasException)
        => this.Exception is null
            ? hasValue.Invoke(this.Value!)
            : hasException.Invoke(Exception!);
}

public class ResultException : Exception
{
    public ResultException() : base("The Result is Failure.") { }
    public ResultException(string message) : base(message) { }

    public static ResultException Create(string message)
        => new ResultException(message);
}
