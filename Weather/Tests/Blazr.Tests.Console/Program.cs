using Blazr.Manganese;
using System.ComponentModel.DataAnnotations;

var inputContainer = Containor<string?>.Read(Console.ReadLine);

Containor<string?>.Read(Console.ReadLine)
    .Write<string?>(Console.WriteLine);

Containor.Read(Console.ReadLine)
    .Write<string?>(Console.WriteLine);

//NullT.Read(Console.ReadLine)
//    .Bind(TryParseToDouble)
//    .Map(Math.Sqrt)
//    .Map(value => Math.Round(value, 2))
//    .Write(
//        hasValue: value => $"Success: The transformed value is: {value}",
//        hasNoValue: () => "The input value could not be parsed.")
//    .Write(Console.WriteLine);



//BoolT.Read(Console.ReadLine)
//    .TryMap(double.Parse)
//    //.Bind(TryParseStringToDouble)
//    .Map(Math.Sqrt)
//    .Map(value => Math.Round(value, 2))
//    .ToIO(
//        hasValue: value => $"Success: The transformed value is: {value}",
//        hasNoValue: () => "The input value could not be parsed.",
//        hasException: ex => $"An error occurred: {ex.Message}")
//    .Write(Console.WriteLine);


//Bool<double> TryParseStringToDouble(string? value)
//{
//    if (double.TryParse(value, out double result))
//        return BoolT.Success(result);

//        return Bool<double>.Failure($"Failed to parse {value}");
//}

//static Null<double> TryParseToDouble(string? value)
//{
//    if (double.TryParse(value, out double result))
//        return NullT.Read(result);

//    return NullT.NoValue<double>();
//}

//double To2Decimals(double value)
//    => Math.Round(value, 2);
//public static class Extensions
//{
//    extension(string? value)
//    {
//        public Bool<string> ToBoolT()
//            => BoolT.Read(value);

//        public Null<double> TryParseToDouble()
//        {
//            if (double.TryParse(value, out double result))
//                return NullT.Read(result);

//            return NullT.NoValue<double>();
//        }
//    }

//    extension(string @this)
//    {
//        public void Write(Action<string> writer)
//            => writer.Invoke(@this);
//    }
//}
