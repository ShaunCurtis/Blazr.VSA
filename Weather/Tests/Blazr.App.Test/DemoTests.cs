/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Manganese;

namespace Blazr.Test;

public class DemoTests
{
    [Fact]
    public void DemoOutput()
    {
        string? value = "Hello Result";
 
        Result<string>.Create(value)
            .OutputResult(
                success: (v) => Console.WriteLine($"Success: {v}"),
                failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
            );

        Result<string>.Create(value)
            .OutputResult(
                failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
            );

        value = null;

        Result<string>.Create(value)
            .OutputResult(
                success: (v) => Console.WriteLine($"Success: {v}"),
                failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
            );
    }

    [Fact]
    public void DemoMap()
    {
        //string? value = "Hello Result";

        //Result<string>.Create(value)
        //    .Map((v) => Result<string>.Create(v.ToUpper))
        //    .OutputResult(
        //        success: (v) => Console.WriteLine($"Success: {v}"),
        //        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
        //    );

        //Result<string>.Create(value)
        //    .Map(ToUpper)
        //    .OutputResult(
        //        success: (v) => Console.WriteLine($"Success: {v}"),
        //        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
        //    );

        //var result = Result<string>.Create(value)
        //  .Map(ToUpper)
        //  .Map();

        //DisplayResult(result);

    }

    private Result<string> ToUpper(string value)
        => string.IsNullOrEmpty(value)
            ? Result<string>.Failure("Value cannot be null or empty")
            : Result<string>.Create(value.ToUpper());

    private void DisplayResult(Result result)
    {
        result.Output(
            failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
        );
    }
}
