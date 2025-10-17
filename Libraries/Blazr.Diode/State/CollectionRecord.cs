/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Collections.Immutable;

namespace Blazr.Diode;

public record CollectionRecord<T, TCollection>
{
    public T Record { get; private init; }
    public ImmutableList<TCollection> Items { get; private init; }

    public CollectionRecord(T record, IEnumerable<TCollection> items)
    {
        this.Record = record;
        this.Items = items.ToImmutableList();
    }
    public bool IsDirty(CollectionRecord<T, TCollection> control)
    {
        if (control.Record is null || !control.Record.Equals(this.Record))
            return true;
        if (control.Items.Count != this.Items.Count)
            return true;
        if (control.Items.Except(this.Items).Count() != 0)
            return true;

        return false;
    }
}
