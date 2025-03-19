/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.App.Invoice.Core;
using Blazr.Gallium;
using MediatR;

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediatr Server Handler that saves an Invoice Aggregate
/// It uses the custom Invoice Database Handler to interface with the database
/// </summary>
public sealed class PersistInvoiceServerHandler : IRequestHandler<InvoiceRequests.InvoiceSaveRequest, Result>
{
    private readonly IMessageBus _messageBus;
    private readonly ICommandBroker<InvoiceComposite> _broker;

    public PersistInvoiceServerHandler(ICommandBroker<InvoiceComposite> broker, IMessageBus messageBus)
    {
        _broker = broker;
        _messageBus = messageBus;
    }

    public async Task<Result> Handle(InvoiceRequests.InvoiceSaveRequest request, CancellationToken cancellationToken)
    {
        var invoice = request.Invoice;

        var result = await _broker.ExecuteAsync(new CommandRequest<InvoiceComposite>(
            Item: invoice,
            State: CommandState.None),
            cancellationToken);

        if (result.HasFailed(out Exception? exception))
            return Result.Fail(exception!);

        _messageBus.Publish<DmoInvoice>(invoice.InvoiceRecord);

        return Result.Success();
    }
}
