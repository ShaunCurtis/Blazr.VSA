/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class CustomerExtensions
{
    extension(CustomerCommandRequest @this)
    {
        public static CustomerCommandRequest Create(CustomerRecordMutor mutor)
            => new CustomerCommandRequest(mutor.Record, mutor.State);
    }
}
