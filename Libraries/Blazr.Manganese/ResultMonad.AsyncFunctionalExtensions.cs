/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result
{
    public async Task<Result> ExecuteFunctionAsync(Func<Task<Result>> function)
        => this.HasException
            ? this
            : await function();

    public async Task<Result<T>> ExecuteFunctionAsync<T>(Func<Task<Result<T>>> function)
        => this.HasException
            ? Result<T>.Failure(this.Exception!)
            : await function();

    public async Task<Result> ExecuteFunctionAsync( bool test, Func<Task<Result>> truefunction, Func<Task<Result>> falsefunction)
        => this.HasException
        ? this
        : test
            ? await truefunction()
            : await falsefunction();
}

