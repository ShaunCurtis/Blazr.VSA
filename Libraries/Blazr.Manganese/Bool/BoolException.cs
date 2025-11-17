/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public class BoolException : Exception
{
    public BoolException() : base("The Result is Failure.") { }
    public BoolException(string message) : base(message) { }

    public static BoolException Create(string message)
        => new BoolException(message);
}

//public class BoolException : Exception
//{
//    public BoolException() : base("The operation Failed.") { }
//    public BoolException(string message) : base(message) { }

//    public static BoolException Create(string message)
//        => new BoolException(message);
//}
