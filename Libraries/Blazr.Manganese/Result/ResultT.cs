/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public abstract record Result<T> { }

public record SuccessResult<T>(T Value) : Result<T>;
public record FailureResult<T>(Exception Exception) : Result<T>;