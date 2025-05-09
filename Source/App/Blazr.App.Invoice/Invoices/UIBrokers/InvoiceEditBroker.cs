/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazored.Toast.Services;
using Blazr.Antimony;
using Blazr.App.Invoice.Core;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.App.Invoice.Presentation;

/// <summary>
/// Provides the Edit context for a Invoice entity in an Invoice Aggregate
/// </summary>
public sealed class InvoiceEditBroker
{
    private readonly IToastService _toastService;
    private readonly InvoiceEntity _invoice;

    public IResult LastResult { get; private set; } = Result.Success();
    public EditContext EditContext { get; private set; }
    public DmoInvoiceEditContext RecordEditContext { get; private set; }
    public bool IsNew => _invoice.InvoiceRecord.Record.Id == InvoiceId.Default;

    public InvoiceEditBroker(InvoiceCompositeBroker invoiceAggregatePresenter, IToastService toastService)
    {
        _invoice = invoiceAggregatePresenter.Invoice;
        _toastService = toastService;
        this.RecordEditContext = new(_invoice.InvoiceRecord.Record);
        this.EditContext = new(this.RecordEditContext);
    }

    /// <summary>
    /// Updates the Invoice entity in the Invoice Aggregate
    /// </summary>
    /// <returns></returns>
    public Task<IResult> SaveItemToAggregateAsync()
    {
        if (!this.RecordEditContext.IsDirty)
        {
            this.LastResult = Result.Failure("The record has not changed and therefore has not been updated.");
            _toastService.ShowWarning("The record has not changed and therefore has not been updated.");
            return Task.FromResult(this.LastResult);
        }

        // get the updated invoice, Need to create a new invoice Id if the Id is the default
        var invoice = this.RecordEditContext.Id == InvoiceId.Default
            ? this.RecordEditContext.AsRecord with { Id = InvoiceId.Create }
            : this.RecordEditContext.AsRecord;

        var result = _invoice.Dispatch(new InvoiceActions.UpdateInvoiceAction(invoice));

        if (result.IsSuccess)
            _toastService.ShowSuccess("The invoice data was updated.");
        else
            _toastService.ShowError(this.LastResult.Message ?? "The invoice data could not be updated.");

        return Task.FromResult(this.LastResult);
    }
}