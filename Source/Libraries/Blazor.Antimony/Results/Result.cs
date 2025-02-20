/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Diagnostics.CodeAnalysis;

namespace Blazr.Antimony.Core;

/// <summary>
/// My Result implementation
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly record struct Result
{
    private Exception? _error { get; init; }
    
    private Result(Exception error)
    {
        IsSuccess = false;
        _error = error;
    }

    //public Result()
    //{
    //    IsSuccess = true;
    //}

    public bool IsSuccess { get; init; } = true;
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Returns true is failure and sets the out item to the exception
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public bool HasFailed([NotNullWhen(true)] out Exception? exception)
    {
        if (this.IsFailure)
            exception = _error;
        else
            exception = default;

        return this.IsFailure;
    }

    /// <summary>
    /// Converts the Result to a UI DataResult
    /// </summary>
    public IDataResult ToDataResult => new DataResult() { Message = _error?.Message, Successful = this.IsSuccess };

    public static Result Success() => new() { IsSuccess = true };
    public static Result Fail(Exception error) => new(error);
}

public readonly record struct Result<T>
{
    // Hidden
    private T? _value { get; init; }
    private Exception? _error { get; init; }

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        _error = null;
    }

    private Result(Exception error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(_error))]
    private bool IsSuccess { get; }
    private bool IsFailure => !IsSuccess;

    /// <summary>
    /// Converts Result<T> to Result
    /// </summary>
    public Result ToResult
        => this.IsSuccess ? Result.Success() : Result.Fail(_error);

    /// <summary>
    /// Returns true is success and sets the out item to T
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HasSucceeded([NotNullWhen(true)] out T? item)
    {
        if (this.IsSuccess)
            item = _value;
        else
            item = default;

        return this.IsSuccess;
    }

    /// <summary>
    /// Returns true is failure and sets the out item to the exception
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public bool HasFailed([NotNullWhen(true)] out Exception? exception)
    {
        if (this.IsFailure)
            exception = _error;
        else
            exception = default;

        return this.IsFailure;
    }

    /// <summary>
    /// The standard Map/Switch method
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Map(Action<T> onSuccess, Action<Exception> onFail)
    {
        if (this.IsSuccess)
            onSuccess(_value);
        else
            onFail(_error);
    }

    /// <summary>
    /// Converts the Result to a UI DataResult
    /// </summary>
    public IDataResult ToDataResult => new DataResult() { Message = _error?.Message, Successful = this.IsSuccess };

    /// <summary>
    /// Converts a failed Result from Result<T> to Result<TOut>
    /// Used in the data pipeline where we map a data object to domain entities
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Result<TOut> ConvertFail<TOut>()
    {
        if (this.IsSuccess)
            throw new InvalidOperationException("You must provide a value if the operation has succeeded.");

        return Result<TOut>.Fail(this._error);
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Fail(Exception error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
}

