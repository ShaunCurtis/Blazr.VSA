using Blazr.Manganese;

string? value = "Hello Result";

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

value = null;

var result = Result<string>.Create(value)
  .ApplyTransform<string>(ToUpper)
  .ToResult;

DisplayError(result);



Result<string> ToUpper(string value)
     => string.IsNullOrEmpty(value)
         ? Result<string>.Failure("Value cannot be null or empty")
         : Result<string>.Create(value.ToUpper());

void DisplayError(Result result)
{
    result.Output(
        hasException: (ex) => Console.WriteLine($"Failure: {ex.Message}")
    );
}


