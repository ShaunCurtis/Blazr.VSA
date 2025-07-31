/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    public static Result<TOut> CreateFromFunction<TOut>(Func<TOut> function)
        => Result.Success().ApplyTransform(function);

    public async  Task<Result<TOut>> ExecuteFunctionAsync<TOut>( Func<T, Task<Result<TOut>>> function)
        => this.HasException
            ? Result<TOut>.Failure(this.Exception!)
            : await function(this.Value!);

    public async  Task<Result<T>> ExecuteFunctionAsync( Func<T, Task<Result<T>>> function)
        => this.HasException
            ? Result<T>.Failure(this.Exception!)
            : await function(this.Value!);

    public async  Task<Result> ExecuteFunctionAsync( Func<T, Task<Result>> function)
        => this.HasException
            ? Result.Failure(this.Exception!)
            : await function(this.Value!);

    public async  Task<Result<TOut>> ExecuteFunctionAsync<TOut>( bool test, Func<T, Task<Result<TOut>>> truefunction, Func<T, Task<Result<TOut>>> falsefunction)
        => test
            ? await this.ExecuteFunctionAsync<TOut>(truefunction)
            : await this.ExecuteFunctionAsync<TOut>(falsefunction);

    public async  Task<Result<T>> ExecuteFunctionAsync( bool test, Func<T, Task<Result<T>>> truefunction)
        => test
            ? await this.ExecuteFunctionAsync<T>(truefunction)
            : this;

    public async  Task<Result> ExecuteFunctionAsync( bool test, Func<T, Task<Result>> truefunction, Func<T, Task<Result>> falsefunction)
        => test
            ? await this.ExecuteFunctionAsync(truefunction)
            : await this.ExecuteFunctionAsync(falsefunction);

    public async  Task<Result> ExecuteFunctionAsync( bool test, Func<T, Task<Result>> truefunction)
        => test
            ? await this.ExecuteFunctionAsync(truefunction)
            : this.ToResult;

    public async Task<Result<TOut>> DispatchAsync<TOut>(Func<T, Task<Result<TOut>>> function)
        => this.HasException
            ? Result<TOut>.Failure(this.Exception!)
            : await function(this.Value!);
}
