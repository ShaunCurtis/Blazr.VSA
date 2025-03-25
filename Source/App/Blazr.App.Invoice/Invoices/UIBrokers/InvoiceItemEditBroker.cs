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

public sealed class InvoiceItemEditBroker
{
    private readonly IToastService _toastService;
    private readonly InvoiceComposite _invoice;
    private InvoiceItemId _invoiceItemId = InvoiceItemId.Default;

    public IResult LastResult { get; private set; } = Result.Success();
    public EditContext EditContext { get; private set; }
    public DmoInvoiceItemEditContext RecordEditContext { get; private set; }
    public bool IsNew { get; private set; }

    public InvoiceItemEditBroker(IToastService toastService, InvoiceCompositeBroker invoiceAggregatePresenter, InvoiceItemId id)
    {
        _invoice = invoiceAggregatePresenter.Invoice;
        _invoiceItemId = id;
        _toastService = toastService;

        // Detect if we have a new item request.
        this.IsNew = id == InvoiceItemId.Default;

        // Get the invoice item from the Invoice Aggregate
        var item = _invoice.Dispatch(new InvoiceActions.GetInvoiceItemAction(id));

        // Create the edit contexts
        RecordEditContext = new(item);
        this.EditContext = new(this.RecordEditContext);
    }

    public Task<IResult> ProcessItemAsync()
    {

        if (!this.RecordEditContext.IsDirty)
        {
            this.LastResult = Result.Failure("The record has not changed and therefore has not been updated.");
            _toastService.ShowWarning("The record has not changed and therefore has not been updated.");
            return Task.FromResult(this.LastResult);
        }

        if (IsNew)
        {
            var AddAction = _invoice.Dispatch(new InvoiceActions.AddInvoiceItemAction(this.RecordEditContext.AsRecord));

            if (AddAction.IsSuccess)
            {
                var message = "The Invoice Item was added to the invoice.";
                _toastService.ShowSuccess(message);
                this.LastResult = Result.Success();
            }
            else
            {
                var message = "The Invoice Item could not be added to the invoice.";
                _toastService.ShowError(message);
                this.LastResult = Result.Failure(message);
            }

            return Task.FromResult(this.LastResult);
        }

        var updateResult = _invoice.Dispatch(new InvoiceActions.UpdateInvoiceItemAction(this.RecordEditContext.AsRecord));
        this.LastResult = updateResult;

        if (updateResult.IsSuccess)
            _toastService.ShowSuccess("The invoice item was updated.");
        else
            _toastService.ShowError(this.LastResult.Message ?? "The Invoice Item could not be added to the invoice.");

        return Task.FromResult(this.LastResult);
    }

    public Task<IResult> DeleteItemAsync()
    {
        if (IsNew)
        {
            var message = "You can't delete an item that you haven't created.";
            _toastService.ShowError(message);
            this.LastResult = Result.Failure(message);

            return Task.FromResult(this.LastResult);
        }

        var deleteResult = _invoice.Dispatch(new InvoiceActions.DeleteInvoiceItemAction(this.RecordEditContext.Id));
        this.LastResult = deleteResult;

        if (this.LastResult.IsSuccess)
            _toastService.ShowSuccess("The invoice item was deleted.");
        else
            _toastService.ShowError(this.LastResult.Message ?? "The Invoice Item could not be deleted from the invoice.");

        return Task.FromResult(this.LastResult);
    }
}