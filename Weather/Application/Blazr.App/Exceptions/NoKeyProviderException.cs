/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class NoKeyProviderException : Exception
{
    public NoKeyProviderException()
        : base($"There's no Key Provider SI Service registered.") { }

    public NoKeyProviderException(string message)
        : base(message) { }
}
