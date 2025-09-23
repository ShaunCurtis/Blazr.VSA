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

    public Result<CollectionRecord<T, TCollection>> AsResult
        => Result<CollectionRecord<T, TCollection>>.Create(this);

    public static CollectionRecord<T, TCollection> Create(T record, IEnumerable<TCollection> items)
        => new(record, items);
}
