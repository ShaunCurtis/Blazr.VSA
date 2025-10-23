//using Blazr.Manganese;

//string? value = Console.ReadLine();

//// monadic function
//await ParseForIntAsync(value)
//    .TaskSideEffectAsync(success: (value) => Console.WriteLine($"Parsed value: {value}"))
//    // Applying a Mapping function
//    .MapTaskToResultAsync(Utilities.ToSquareRoot)
//    // Output the result
//    .OutputTaskAsync(
//        success: (value) => Console.WriteLine($"Success: {value}"),
//        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
//    );


//ParseForInt(value)
//    // Get out an intermediate result
//    .ApplySideEffect(success: (value) => Console.WriteLine($"Parsed value: {value}"))
//    // Applying a Mapping function
//    .ApplyTransform(Utilities.ToSquareRoot)
//    // Output the result
//    .OutputResult(
//        success: (value) => Console.WriteLine($"Success: {value}"),
//        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
//    );

////Result<string>.Create(value)
////    .Output(
////        success: (v) => Console.WriteLine($"Success: {v}"),
////        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
////    );

////Result<string>.Create(value)
////    .Output(
////        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
////    );

////value = null;

////Result<string>.Create(value)
////    .Output(
////        success: (v) => Console.WriteLine($"Success: {v}"),
////        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
////    );

////Result<string>.Create(value)
////    .Map((v) => Result<string>.Create(v.ToUpper()))
////    .Output(
////        success: (v) => Console.WriteLine($"Success: {v}"),
////        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
////    );

////Result<string>.Create(value)
////    .Map(ToUpper)
////    .Output(
////        success: (v) => Console.WriteLine($"Success: {v}"),
////        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
////    );

////value = null;

////Result<string>
////    .Create(value)
////    .OutputResult(
////        success: (value) => Console.WriteLine($"Success: {value}"),
////        failure: (exception) => Console.WriteLine($"Failure: {exception.Message}")
////    );

////var result = Result<string>.Create(value)
////  .ApplyTransform(ToUpper)
////  .ApplyTransform();

////DisplayError(result);

//Result<double> SquareRoot(int value)
//    => Result<double>.Create(Math.Sqrt(value));

//Result<int> ParseForInt(string? value)
//    => int.TryParse(value, out int result)
//        ? Result<int>.Create(result)
//        : Result<int>.Failure(new FormatException("Input is not a valid integer."));

//async Task<Result<int>> ParseForIntAsync(string? input)
//{ 
//    await Task.Yield(); // Simulate async operation
//    return int.TryParse(input, out int result)
//        ? Result<int>.Create(result)
//        : Result<int>.Failure(new FormatException("Input is not a valid integer."));
//}

//Result<string> ToUpper(string value)
//     => string.IsNullOrEmpty(value)
//         ? Result<string>.Failure("Value cannot be null or empty")
//         : Result<string>.Create(value.ToUpper());

//void DisplayError(Result result)
//{
//    result.Output(
//        failure: (ex) => Console.WriteLine($"Failure: {ex.Message}")
//    );
//}

//public class myClass
//{
//    Func<string, Result<int>> ParseForInt => (string input) =>
//    {
//        var intResult = int.Parse(input);
//        return Result<int>.Create(intResult);
//    };
//}


//public static class Utilities
//{
//    public static Func<int, Result<double>> ToSquareRoot => (value)
//        => Result<double>.Create(Math.Sqrt(value));
//}
