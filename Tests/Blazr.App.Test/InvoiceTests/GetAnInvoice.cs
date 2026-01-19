/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Core.Invoices;
using Blazr.Diode.Mediator;
using Blazr.Manganese;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class InvoiceTests
{

    [Fact]
    public async Task GetAnInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();

        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Invoices.Skip(Random.Shared.Next(3)).First();
        var controlRecord = this.AsDmoInvoice(controlItem);
        var controlId = controlRecord.Id;
        var _controlInvoiceItems = _testDataProvider.InvoiceItems.Where(item => item.InvoiceID == controlItem.InvoiceID);
        var controlInvoiceItems = _controlInvoiceItems.Select(item => this.AsDmoInvoiceItem(item)).ToList();

        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(controlId));

        Assert.True(entityResult.HasValue);
        Assert.Equal(controlInvoiceItems.Count, entityResult.AsSuccess.Value.InvoiceItems.Count);
        Assert.Contains(entityResult.AsSuccess.Value.InvoiceItems.First(), controlInvoiceItems);
    }
}
