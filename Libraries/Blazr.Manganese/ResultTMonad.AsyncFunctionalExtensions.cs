/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    public async  Task<Result<TOut>> ApplyTransformAsync<TOut>( Func<T, Task<Result<TOut>>> transform)
        => this.HasException
            ? Result<TOut>.Failure(this.Exception!)
            : await transform(this.Value!);

    public async  Task<Result<T>> ApplyTransformAsync( Func<T, Task<Result<T>>> transform)
        => this.HasException
            ? Result<T>.Failure(this.Exception!)
            : await transform(this.Value!);

    public async  Task<Result> ApplyTransformAsync( Func<T, Task<Result>> transform)
        => this.HasException
            ? Result.Failure(this.Exception!)
            : await transform(this.Value!);

    public async  Task<Result<TOut>> ApplyTransformAsync<TOut>( bool test, Func<T, Task<Result<TOut>>> trueTransform, Func<T, Task<Result<TOut>>> falseTransform)
        => test
            ? await this.ApplyTransformAsync<TOut>(trueTransform)
            : await this.ApplyTransformAsync<TOut>(falseTransform);

    public async  Task<Result<T>> ApplyTransformAsync( bool test, Func<T, Task<Result<T>>> trueTransform)
        => test
            ? await this.ApplyTransformAsync<T>(trueTransform)
            : this;

    public async  Task<Result> ApplyTransformAsync( bool test, Func<T, Task<Result>> trueTransform, Func<T, Task<Result>> falseTransform)
        => test
            ? await this.ApplyTransformAsync(trueTransform)
            : await this.ApplyTransformAsync(falseTransform);

    public async  Task<Result> ApplyTransformAsync( bool test, Func<T, Task<Result>> trueTransform)
        => test
            ? await this.ApplyTransformAsync(trueTransform)
            : this.ToResult;

    public async Task<Result<TOut>> DispatchAsync<TOut>(Func<T, Task<Result<TOut>>> transform)
        => this.HasException
            ? Result<TOut>.Failure(this.Exception!)
            : await transform(this.Value!);
}
