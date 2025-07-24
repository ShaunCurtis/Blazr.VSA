/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Cadmium.Core;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.Cadmium.Presentation;

public partial class WeatherForecastEntityEditUIBroker
{
    public WeatherForecastId Id => _entity?.Id ?? WeatherForecastId.Default;

    public Result LastResult { get; protected set; } = Result.Success();

    public WeatherForecastEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public WeatherForecastEntityEditUIBroker(IEntityProvider<DmoWeatherForecast, WeatherForecastId> entityProvider)
    {
        _entityProvider = entityProvider as WeatherForecastEntityProvider ?? throw new Exception("The provided EntityProvider is not a WeatherForecastEntityProvider ");

        this.EditContext = new EditContext(EditMutator);
    }

    public async ValueTask LoadAsync(WeatherForecastId id)
    {
        LastResult = await Result<WeatherForecastId>.Create(id)
            .ApplyTransformAsync((id) => _isLoaded ? Task.FromResult(LoadedResult) : this.LoadEntityAsync(id));
    }

    public ValueTask ResetItemAsync()
    {
        LastResult = Result.Success()
            .ApplyTransform(() => _isLoaded ? Result.Success() : NotLoadedResult)
            .ApplyTransform(this.ResetEntity)
            .ApplySideEffect(hasNoException: () =>
                    {
                        // Reset the EditMutator and create a new EditContext to rebuild the whole Edit Form
                        EditMutator.Reset();
                        this.EditContext = new EditContext(EditMutator);
                    });

        return ValueTask.CompletedTask;
    }

    public async ValueTask SaveItemAsync(bool refreshOnNew = true)
    {
        LastResult = await Result.Success()
            // check if the UIBroker is loaded and only update if it is
            .ApplyTransformAsync(() => _isLoaded ? this.UpdateEntityAsync(refreshOnNew) : Task.FromResult(NotLoadedResult))
            .ApplySideEffectAsync(hasValue: () =>
            {
                WeatherForecastEntity.MarkAsPersistedAction
                    .CreateAction()
                    .AddSender(this)
                    .ExecuteAction(_entity);
            });
    }

    public async ValueTask DeleteItemAsync()
    {
        LastResult = await Result.Success()
            .ApplyTransformAsync(() => _isLoaded ? this.DeleteEntityAsync() : Task.FromResult(NotLoadedResult));
    }
}

public partial class WeatherForecastEntityEditUIBroker
{
    private readonly WeatherForecastEntityProvider _entityProvider;
    private WeatherForecastEntity _entity = default!;
    private bool _isLoaded;

    private static Result NotLoadedResult
        => Result.Failure("The UIBroker has not been loaded. There is nothing to save.");

    private static Result LoadedResult
        => Result.Failure("The UIBroker has already been loaded. You can not reload it.");

    private async Task<Result> LoadEntityAsync(WeatherForecastId id)
        => await Result<WeatherForecastId>.Create(id)
            .ApplyTransformAsync<WeatherForecastEntity>(_entityProvider.EntityRequestAsync)
            .ApplySideEffectAsync(
                hasValue: (entity) =>
                {
                    _entity = entity;
                    this.EditMutator = new();
                    this.EditMutator.Load(entity.WeatherForecast);
                    this.EditContext = new EditContext(EditMutator);
                    _isLoaded = true;
                })
            .ToResultAsync();

    private async Task<Result> UpdateEntityAsync(bool refreshOnNew = true)
         => await WeatherForecastEntity.UpdateWeatherForecastAction
            // Update the entity with the values from the EditMutator
            .CreateAction(EditMutator.AsRecord)
            .AddSender(this)
            .ExecuteAction(_entity)
            // Persist the update to the data store
            .ApplyTransformAsync(_entityProvider.EntityCommandAsync)
            // If the update was successful, we need to reload the entity
            //.MapTaskAsync<WeatherForecastId, WeatherForecastEntity>(_entityProvider.EntityRequestAsync)
            .ToResultAsync();

    private Result ResetEntity()
        => WeatherForecastEntity.ResetAction
            // Set the entity as deleted
            .CreateAction()
            .AddSender(this)
            .ExecuteAction(_entity);

    private async Task<Result> DeleteEntityAsync()
        => await WeatherForecastEntity.DeleteWeatherForecastAction
            // Set the entity as deleted
            .CreateAction()
            .AddSender(this)
            .ExecuteAction(_entity)
            // Persist the deletion to the data store
            .ApplyTransformAsync(_entityProvider.EntityCommandAsync)
            .ToResultAsync();
}
