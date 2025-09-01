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
        LastResult = await this.IsNotLoaded
            .ExecuteTransform(id.ToResult)
            .ExecuteTransformAsync<WeatherForecastEntity>(_entityProvider.EntityRequestAsync)
            .ExecuteTransformAsync(this.LoadBroker);
    }


    public void Reset()
    {
        LastResult = this.IsLoaded
            .ExecuteTransform(() => WeatherForecastEntity.ResetAction
                    .CreateAction()
                    .AddSender(this)
                    .ExecuteAction(_entity))
            .ExecuteActionOnSuccess((entity) => 
            {
                EditMutator.Reset();
                this.EditContext = new EditContext(EditMutator);
            })
            .ToResult();
    }

    public async ValueTask SaveAsync(bool refreshOnNew = true)
    {
        LastResult = await this.IsLoaded
            .ExecuteTransaction(this.UpdateWeatherForecast)
            .ExecuteFunctionAsync(this.SaveEntityAsync);
    }

    public async ValueTask DeleteAsync()
    {
        LastResult = await this.IsLoaded
            .ExecuteTransform(() => WeatherForecastEntity.DeleteWeatherForecastAction
                .CreateAction()
                .AddSender(this)
                .ExecuteAction(_entity))
            .ExecuteTransformAsync(_entityProvider.EntityCommandAsync)
            .ToResultAsync();
    }
}

public partial class WeatherForecastEntityEditUIBroker
{
    private readonly WeatherForecastEntityProvider _entityProvider;
    private WeatherForecastEntity _entity = default!;
    private bool _isLoaded;

    private Result IsLoaded
        => Result.Success(_isLoaded, "The UIBroker has not been loaded. There is nothing to save.");

    private Result IsNotLoaded
        => Result.Failure(_isLoaded, "The UIBroker has already been loaded. You can not reload it.");

    private Result LoadBroker(WeatherForecastEntity entity)
    {
        _entity = entity;
        this.EditMutator = new();
        this.EditMutator.Load(entity.WeatherForecast);
        this.EditContext = new EditContext(EditMutator);
        _isLoaded = true;
        return Result.Success();
    }

    private Result UpdateWeatherForecast()
         => WeatherForecastEntity.UpdateWeatherForecastAction
            // Update the entity with the values from the EditMutator
            .CreateAction(EditMutator.AsRecord)
            .AddSender(this)
            .ExecuteAction(_entity)
            .ToResult();

    private async Task<Result> SaveEntityAsync()
         => await WeatherForecastEntity.SaveEntityAction
        .CreateAction(_entityProvider.EntityCommandAsync)
        .AddSender(this)
        .ExecuteActionAsync(_entity)
        .ToResultAsync();
}
