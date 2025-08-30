using Blazr.Diode;
using System;
using System.Reflection;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public partial class WeatherForecastEntity
{
    public record SaveEntityAction : BaseAction<SaveEntityAction>
    {
        private Func<WeatherForecastEntity, Task<Result<WeatherForecastEntity>>> _persistFunction;
        public SaveEntityAction(Func<WeatherForecastEntity, Task<Result<WeatherForecastEntity>>> persistFunction) 
        {
            _persistFunction = persistFunction;
        }

        public static SaveEntityAction CreateAction(Func<WeatherForecastEntity, Task<Result<WeatherForecastEntity>>> _persistFunction)
            => new(_persistFunction) { Sender = null };

        public SaveEntityAction WithSender(object sender)
            => this with { Sender = sender };

        public async Task<Result<WeatherForecastEntity>> ExecuteActionAsync(WeatherForecastEntity entity)
            => await _persistFunction.Invoke(entity)
            .ExecuteActionOnSuccessAsync((value) => entity._weatherForecast.MarkAsPersisted())
            .ExecuteActionOnSuccessAsync((value) => entity.StateHasChanged?.Invoke(this.Sender, value.Id));
    }
}
