/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class InvalidKeyProviderException : Exception
{
    public InvalidKeyProviderException()
        : base($"The provided key object is not the right type.") { }

    public InvalidKeyProviderException(string message)
        : base(message) { }
}
