﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;

namespace Blazr.Antimony;

/// <summary>
/// My Result implementation
/// </summary>
public record Result : IResult
{
    private readonly Exception? _error;

    [MemberNotNullWhen(false, nameof(_error))]
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    
    private Result()
    {
        IsSuccess = true;
        _error = null;
    }

    private Result(Exception error)
    {
        IsSuccess = false;
        _error = error;
    }

    /// <summary>
    /// Returns true is failure and sets the out item to the exception
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public bool HasFailed([NotNullWhen(true)] out Exception? exception)
    {
        exception = default;

        if (this.IsFailure)
            exception = _error;

        return this.IsFailure;
    }

    /// <summary>
    /// The standard Map/Switch method
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Map(Action onSuccess, Action<Exception> onFail)
    {
        if (this.IsSuccess)
            onSuccess();
        else
            onFail(_error);
    }

    string? IResult.Message => _error?.Message;

    /// <summary>
    /// Static Success constructor
    /// </summary>
    /// <returns></returns>
    public static Result Success() => new();

    /// <summary>
    /// Static Fail constructor
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result Fail(Exception error) => new(error);

    /// <summary>
    /// Static Failure constructor
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static Result Failure(string message) => new(new ResultException(message));

}

/// <summary>
/// My Result<T> implementation
/// </summary>
/// <typeparam name="T"></typeparam>
public record Result<T> : IResult
{
    // Hidden
    private readonly T? _value;
    private readonly Exception? _error;

    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(_error))]
    public bool IsSuccess { get; private init; }

    private bool IsFailure => !IsSuccess;

    /// <summary>
    /// Private constructor for success
    /// </summary>
    /// <param name="value"></param>
    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        _error = null;
    }

    /// <summary>
    /// Private constructor for failure
    /// </summary>
    /// <param name="error"></param>
    private Result(Exception error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

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

    public bool HasNotSucceeded([NotNullWhen(false)] out T? item)
    {
        if (this.IsSuccess)
            item = _value;
        else
            item = default;

        return this.IsFailure;
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

    string? IResult.Message => _error?.Message;

    /// <summary>
    /// Static Success constructor
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// static Fail constructor
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<T> Fail(Exception error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
}

