using Blazr.Manganese;

Console
    .ReadLine()
    .ParseForInt()
    .ApplyTransform((value) => Math.Sqrt(value))
    .ApplyTransform((value) => Math.Round(value, 2))
    //  Handle the transforms
    .Output(
        hasValue: (value) => Console.WriteLine($"Parsed successfully: The transformed value is: {value}"),
        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
    );

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


public static class stringExtensions
{
    public static Result<int> ParseForInt(this string? input) =>
        int.TryParse(input, out int value)
        ? Result<int>.Create(value)
        : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
    //    : Result<int>.Failure(new ResultException($"{input ?? "Null"} was not a valid integer"));
}


//bool isInt = int.TryParse(input, out int value);

//// apply some transforms to the result of the parsing
//double result = 0;
//if (isInt)
//{
//    result = Math.Sqrt(value);
//    result = Math.Round(result, 2);
//}
////... later
//if (isInt)
//{
//    Console.WriteLine($"Parsed successfully: The transformed value of {value} is: {result}");
//}
//else
//{
//    Console.WriteLine($"Failed to parse input: {input}");
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

