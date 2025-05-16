/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using static Blazr.App.Invoice.Core.InvoiceActions;

namespace Blazr.App.Invoice.Core;
public static partial class InvoiceActions
{
    public readonly record struct ResetInvoiceAction();
}

/// <summary>
/// Resets the Invoice to the base Invoice
/// </summary>
/// <returns></returns>
public sealed partial class InvoiceEntity
{
    public Result Dispatch(ResetInvoiceAction action)
    {
        var result = this.ResetInvoice();

        return result;
    }

    private Result ResetInvoice()
    {
        _items.Clear();
        _itemsBin.Clear();

        foreach (var item in _baseItems)
        {
            _items.Add(new InvoiceItemContext(item with { }));
        }

        this.ApplyRules();

        this.Invoice.Update(_baseInvoice);

        return Result.Success();
    }
}
