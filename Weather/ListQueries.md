# Data Pipeline List Queries

Within Grid Forms you will find this:

```csharp
    public async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest)
    {
        //sort the GridState

        var result = await this.UIBroker.GetItemsAsync();

        return result;
    }
```

And within the the GridUIBroker you will find the following:

```csharp
public ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync()
    => new ValueTask<GridItemsProviderResult<TRecord>>( 
        this.GridState.ToResult()
            .ApplyTransformAsync(_entityProvider.GetItemsAsync)
            .ApplySideEffectAsync((result) => this.LastResult = result)
            .OutputAsync(ExceptionOutput: (ex) => GridItemsProviderResult.From<TRecord>(new List<TRecord>(), 0)));
```

Steps:
1. The `GridState` is updated by the Grid Form.
1. 