using Blazr.Manganese;


Console.WriteLine(
    Console.ReadLine()
);

//Console.WriteLine(
//    ConsoleHelper.ReadLine()
//    .Bind(TryParseString)
//    //.TryMap(double.Parse)
//    .Map(Math.Sqrt)
//    .Map(value => Math.Round(value, 2))
//    .Map<string>(value => $"Success: The transformed value is: {value}")
//    .OutputValue(defaultValue: "The input value could not be parsed.")
//);

BoolT.Read(Console.ReadLine)
    .Bind(TryParseString)
    .Map(Math.Sqrt)
    .Map(value => Math.Round(value, 2))
    .ToIOMonad(
        hasValue: value => $"Success: The transformed value is: {value}",
        hasNoValue: () => "The input value could not be parsed.",
        hasException: ex => $"An error occurred: {ex.Message}")
    .Output(Console.WriteLine);

//Console.WriteLine(
//    ConsoleHelper.ReadLine()
//    .Bind(TryParseString)
//    .Map(Math.Sqrt)
//    .Map(value => Math.Round(value, 2))
//    .Match<double, string>(
//        hasValue: value => $"Success: The transformed value is: {value}",
//        hasNoValue: () => "The input value could not be parsed.",
//        hasException: ex => $"An error occurred: {ex.Message}"
//    )
//);


//Console.WriteLine(
//    ConsoleHelper.ReadLine()
//    .TryMap(double.Parse)
//    .Map(Math.Sqrt)
//    .Map(value => Math.Round(value, 2))
//    .Map<string>(value => $"Success: The transformed value is: {value}")
//    .OutputValue(defaultValue: "The input value could not be parsed.")
//);

//{
//    var input = Console.ReadLine();
//    Console.WriteLine(input);
//}

//IOMonad.Input(Console.ReadLine())
//    .Map(value => value?.Trim() ?? null)
//    .Bind( value => IOMonad.Input(value?.ToUpper() ?? "[Null]"))
//    .Bind(value => IOMonad.Input($"The Value is:{value}"))
//    .Output(Console.WriteLine);

//{
//    var input = Console.ReadLine();
//    if (double.TryParse(input, out double result))
//    {
//        result = Math.Sqrt(result);
//        result = Math.Round(result, 2);
//        Console.WriteLine($"The result is {result}");

//    }
//    else
//        Console.WriteLine("The input value could not be parsed.");
//}

//{
//    var input = Console.ReadLine();
//    try
//    {
//        var result = double.Parse(input!);
//        result = Math.Sqrt(result);
//        result = Math.Round(result, 2);
//        Console.WriteLine($"The result is {result}");
//    }
//    catch
//    {
//        Console.WriteLine("The input value could not be parsed.");
//    }
//}

//try
//{
//    Console.WriteLine(double.Parse(Console.ReadLine()!));
//}
//catch {
//    Console.WriteLine("It looks like things went BANG!");
//}

//  Console.WriteLine(
//    Console.ReadLine()
//    .ToResult()
//    .MapFunction(Double.Parse)
//    .MapFunction(Math.Sqrt)
//    .BindFunction(To2Decimals)
//    .OutputValue<string>(
//        hasValue: (value) => $"Success: The transformed value is: {value}",
//        hasException: (ex) => $"Failure: {ex.Message}"
//    ));


Bool<double> TryParseString(string? value)
{
    if (double.TryParse(value, out double result))
        return BoolT.Success(result);

        return Bool<double>.Failure($"Failed to parse {value}");
}

//Result<string> To2Decimals(double value)
//    => Result<string>.Create(Math.Round(value, 2).ToString());

//Console.WriteLine(
//    Result<string>
//        .ExecuteFunction<string>(Console.ReadLine)
//        .OutputValue<string>(
//            hasValue: (value) => $"Success: The transformed value is: {value}",
//            hasException: (ex) => $"Failure: {ex.Message}"
//        )
//    );

//Console
//    .ReadLine()
//    .ParseForInt()
//    .ExecuteFunction((value) => Math.Sqrt(value))
//    .ExecuteFunction((value) => Math.Round(value, 2))
//    //  Handle the functions
//    .Output(
//        hasValue: (value) => Console.WriteLine($"Parsed successfully: The functioned value is: {value}"),
//        hasException: (ex) => Console.WriteLine($"Failed to parse input: {ex.Message}")
//    );

//... functions


//Console.ReadLine()
//   .ToResult()
//   .ExecuteFunction<double>(double.Parse)
//   .Output(
//        hasValue: (value) => Console.WriteLine($"Value is: {value}"),
//        hasException: (exception) => Console.WriteLine($"Error: {exception.Message}")
//    );

//Console.WriteLine(
//    Console.ReadLine()
//   .ToResult()
//   .ExecuteFunction<double>(double.Parse)
//   .ExecuteFunction(Math.Sqrt)
//   .ExecuteFunction((value) => Math.Round(value, 2))
//   .OutputValue<string>(
//        hasValue: (value) => $"Value is: {value}",
//        hasException: (exception) => $"Error: {exception.Message}"
//    ));
public static class Extensions
{
    extension(string? value)
    {
        public Bool<string> ToBoolT()
            => BoolT.Read(value);
    }
}

public static class ConsoleHelper
{
    public static Bool<string> ReadLine()
    {
        string? input = Console.ReadLine();
        return BoolT.Read(input);
    }
}
