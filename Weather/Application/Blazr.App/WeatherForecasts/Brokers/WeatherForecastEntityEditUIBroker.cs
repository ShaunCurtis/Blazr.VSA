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
            .MapToResultAsync(
                test: _isLoaded,
                isTrue: id => Task.FromResult(LoadedResult),
                isFalse: id => this.LoadEntityAsync(id));
    }

    public ValueTask ResetItemAsync()
    {
        LastResult = Result.Success()
            .MapToResult(
                test: _isLoaded,
                isTrue: () => Result.Success(),
                isFalse: () => NotLoadedResult
            )
            .MapToResult(this.ResetEntity)
            .SideEffect(
                success: () =>
                    {
                        // Reset the EditMutator
                        EditMutator.Reset();
                        // Create a new EditContext.
                        // This will reset and rebuild the whole Edit Form
                        this.EditContext = new EditContext(EditMutator);
                    }
                );

        return ValueTask.CompletedTask;
    }

    public async ValueTask SaveItemAsync(bool refreshOnNew = true)
    {
        LastResult = await Result.Success()
            // check if the UIBroker is loaded and only update if it is
            .MapToResultAsync(
                test: _isLoaded,
                isTrue: () => this.UpdateEntityAsync(refreshOnNew),
                isFalse: () => Task.FromResult(NotLoadedResult))
            // if successful, we need sort the entity internal state as persisted
            .TaskSideEffectAsync( success:() =>  {
                WeatherForecastEntity.MarkAsPersistedAction
                    .CreateAction()
                    .AddSender(this)
                    .ExecuteAction(_entity);
            } );
    }

    public async ValueTask DeleteItemAsync()
    {
        LastResult = await Result.Success()
            .MapToResultAsync(
                test: _isLoaded,
                isTrue: () => this.DeleteEntityAsync(),
                isFalse: () => Task.FromResult(NotLoadedResult));
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
            .MapToResultAsync<WeatherForecastEntity>(_entityProvider.EntityRequestAsync)
            .TaskSideEffectAsync(
                success: (entity) =>
                {
                    _entity = entity;
                    this.EditMutator = new();
                    this.EditMutator.Load(entity.WeatherForecast);
                    this.EditContext = new EditContext(EditMutator);
                    _isLoaded = true;
                })
            .MapTaskToResultAsync();

    private async Task<Result> UpdateEntityAsync(bool refreshOnNew = true)
        => await WeatherForecastEntity.UpdateWeatherForecastAction
            // Update the entity with the values from the EditMutator
            .CreateAction(EditMutator.AsRecord)
            .AddSender(this)
            .ExecuteAction(_entity)
            // Persist the update to the data store
            .MapToResultAsync(_entityProvider.EntityCommandAsync)
            // If the update was successful, we need to reload the entity
            //.MapTaskAsync<WeatherForecastId, WeatherForecastEntity>(_entityProvider.EntityRequestAsync)
            .MapTaskToResultAsync();

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
            .MapToResultAsync(_entityProvider.EntityCommandAsync)
            .MapTaskToResultAsync();
}
