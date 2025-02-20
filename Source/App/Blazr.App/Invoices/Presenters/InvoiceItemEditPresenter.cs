/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.App.Presentation;

public sealed class InvoiceItemEditPresenter
{
    private readonly IToastService _toastService;
    private readonly InvoiceWrapper _invoice;
    private InvoiceItemId _invoiceItemId = InvoiceItemId.Default;

    public IDataResult LastResult { get; private set; } = DataResult.Success();
    public EditContext EditContext { get; private set; }
    public DmoInvoiceItemEditContext RecordEditContext { get; private set; }
    public bool IsNew { get; private set; }

    public InvoiceItemEditPresenter(IToastService toastService, InvoiceAggregatePresenter invoiceAggregatePresenter, InvoiceItemId id)
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

    public Task<IDataResult> ProcessItemAsync()
    {

        if (!this.RecordEditContext.IsDirty)
        {
            this.LastResult = DataResult.Failure("The record has not changed and therefore has not been updated.");
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
                this.LastResult = DataResult.Success(message);
            }
            else
            {
                var message = "The Invoice Item could not be added to the invoice.";
                _toastService.ShowError(message);
                this.LastResult = DataResult.Failure(message);
            }

            return Task.FromResult(this.LastResult);
        }

        var updateResult = _invoice.Dispatch(new InvoiceActions.UpdateInvoiceItemAction(this.RecordEditContext.AsRecord));
        this.LastResult = updateResult.ToDataResult;

        if (updateResult.IsSuccess)
            _toastService.ShowSuccess("The invoice item was updated.");
        else
            _toastService.ShowError(this.LastResult.Message ?? "The Invoice Item could not be added to the invoice.");

        return Task.FromResult(this.LastResult);
    }

    public Task<IDataResult> DeleteItemAsync()
    {
        if (IsNew)
        {
            var message = "You can't delete an item that you haven't created.";
            _toastService.ShowError(message);
            this.LastResult = DataResult.Failure(message);

            return Task.FromResult(this.LastResult);
        }

        var deleteResult = _invoice.Dispatch(new InvoiceActions.DeleteInvoiceItemAction(this.RecordEditContext.Id));
        this.LastResult = deleteResult.ToDataResult;

        if (this.LastResult.Successful)
            _toastService.ShowSuccess("The invoice item was deleted.");
        else
            _toastService.ShowError(this.LastResult.Message ?? "The Invoice Item could not be deleted from the invoice.");

        return Task.FromResult(this.LastResult);
    }
}