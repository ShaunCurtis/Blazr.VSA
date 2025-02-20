/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

/// <summary>
/// Base imnplementation of IDataResult
/// Used in the UI to display the message (if failure)
/// </summary>
public readonly record struct DataResult : IDataResult
{
    public bool Successful { get; init; } = true;
    public string? Message { get; init; }

    public DataResult() { }

    public static DataResult Success(string? message = null)
    {
        return new DataResult { Successful = true, Message = message };
    }

    public static DataResult Failure(string message)
    {
        return new DataResult { Message = message };
    }
}
