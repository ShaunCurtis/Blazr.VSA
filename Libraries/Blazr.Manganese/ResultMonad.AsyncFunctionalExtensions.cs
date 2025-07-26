/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result
{
    public async Task<Result> ApplyTransformAsync(Func<Task<Result>> transform)
        => this.HasException
            ? this
            : await transform();

    public async Task<Result<T>> ApplyTransformAsync<T>(Func<Task<Result<T>>> transform)
        => this.HasException
            ? Result<T>.Failure(this.Exception!)
            : await transform();

    public async Task<Result> ApplyTransformAsync( bool test, Func<Task<Result>> trueTransform, Func<Task<Result>> falseTransform)
        => this.HasException
        ? this
        : test
            ? await trueTransform()
            : await falseTransform();
}

