/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;

namespace Blazr.App.Invoice.Core;

public sealed partial class InvoiceEntity
{
    /// <summary>
    /// Resets the Invoice to the base Invoice
    /// </summary>
    /// <returns></returns>
    public Result ResetInvoice()
    {
        _items.Clear();
        _itemsBin.Clear();

        foreach (var item in _baseItems)
        {
            _items.Add(new InvoiceItem(item with { }));
        }

        this.ApplyRules();

        this.Invoice.Update(_baseInvoice);

        return Result.Success();
    }
}
