using Blazr.Manganese;
using System;

Console.WriteLine(
    Result<string>
        .CreateFromTransform(Console.ReadLine)
        .OutputValue<string>(
            hasValue: (value) => $"Success: The transformed value is: {value}",
            hasException: (ex) => $"Failure: {ex.Message}"
        )
    );

//Console
//    .ReadLine()
//    .ParseForInt()
//    .ApplyTransform((value) => Math.Sqrt(value))
//    .ApplyTransform((value) => Math.Round(value, 2))
//    //  Handle the transforms
//    .Output(
//        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value is: {value}"),
//        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
//    );

//... transforms



//var input = Console.ReadLine();

//var parseResult = ParseForInt(input);

//if (int.TryParse(input, out int value))
//{
//    parseResult = Result<int>.Create(value);
//}
//else
//{
//    parseResult = Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
//}

//var parseResult = int.TryParse(input, out int value)
//    ? Result<int>.Create(value)
//    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));

//if (parseResult.HasValue)
//{
//    double transformedValue = Math.Sqrt(parseResult.Value);
//    transformedValue = Math.Round(transformedValue, 2);
//    Console.WriteLine($"Parsed successfully: The transformed value is: {transformedValue}");
//}
//else
//{
//    Console.WriteLine(parseResult.Exception!.Message);
//}

//Result<int> ParseForInt(string? input) =>
//    int.TryParse(input, out int value)
//    ? Result<int>.Create(value)
//    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));


//double doubleValue = 0;

//await Console
//    .ReadLine()
//    .ToResult()
//    .ApplyTransformAsync(Utils.StringToDoubleAsync)
//    .MutateStateAsync((value) => doubleValue = value)
//    .ApplyTransformAsync(Math.Sqrt)
//    .ApplyTransformAsync((value) => Math.Round(value, 2))
//    .OutputAsync(
//        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value of {doubleValue} is: {value}"),
//        hasException: (ex) => Console.WriteLine($"Failed: {ex.Message}")
//    );

//if (result.HasValue)
//{
//    //...
//    Console.WriteLine($"Parsed successfully: The transformed value is: {result.Value}");
//}
//else
//{
//    Console.WriteLine($"Failed to parse input: {result.Exception!.Message}");
//}

Result<int> ParseForInt(string input)
{
    if (!string.IsNullOrEmpty(input))
    {
        try
        {
            var value = int.Parse(input!);

            var result = Math.Sqrt(value);
            result = Math.Round(result, 2);
            return Result<int>.Create((int)result);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(ex);
        }
    }
    else
    {
        return Result<int>.Failure("Input cannot be null or empty.");
    }
}

//if (!string.IsNullOrEmpty(input))
//{
//    try
//    {
//        var value = int.Parse(input!);

//        var result = Math.Sqrt(value);
//        result = Math.Round(result, 2);
//        Console.WriteLine($"Parsed successfully: The transformed value of {value} is: {result}");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"An exception occurred: {ex.Message}");
//    }
//}
//else
//{
//    Console.WriteLine("Input cannot be null or empty.");
//}



//Result<string>.Create(value)
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//Result<string>.Create(value)
//    .Output(
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//value = null;

//Result<string>.Create(value)
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//Result<string>.Create(value)
//    .Map((v) => Result<string>.Create(v.ToUpper()))
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );

//Result<string>.Create(value)
//    .Map(ToUpper)
//    .Output(
//        success: (v) => Console.WriteLine($"Success: {v}"),
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );


public static class Utils
{
    public static Func<string?, Task<Result<double>>> StringToDoubleAsync = async (input)
        =>
    {
        await Task.Yield();
        return double.TryParse(input, out double value)
        ? Result<double>.Create(value)
        : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    };

    public static async Task<Result<double>> ParseForDoubleAsync(this string? input)
    {
        await Task.Yield();
        return double.TryParse(input, out double value)
        ? Result<double>.Create(value)
        : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    }
}


public static class stringExtensions
{
    public static Result<string> ToResult(this string? input) =>
        Result<string>.Create(input);

    public static async Task<Result<double>> ParseForDouble(this string? input)
    {
        await Task.Yield();
        return double.TryParse(input, out double value)
        ? Result<double>.Create(value)
        : Result<double>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    }

    public static Result<int> ParseForInt(this string? input) =>
        int.TryParse(input, out int value)
        ? Result<int>.Create(value)
        : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    //    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
}


//var xresult = Result<string>.Create(input)
//  .ApplyTransform<string>(ToUpper)
//  .ToResult;

//DisplayError(xresult);



//Result<string> ToUpper(string value)
//     => string.IsNullOrEmpty(value)
//         ? Result<string>.Failure("Value cannot be null or empty")
//         : Result<string>.Create(value.ToUpper());

//void DisplayError(Result result)
//{
//    result.Output(
//        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );
//}

