/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;
using static Blazr.App.Weather.Core.WeatherForecastActions;

namespace Blazr.App.Weather.Core;

public static partial class WeatherForecastActions
{
    public readonly record struct ApplyRulesAction(object? sender = null)
    {
        public static ApplyRulesAction Empty => new ApplyRulesAction();
    }
}

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Applies the business rules to the Weather Forecast
    /// </summary>
    /// <param name="sender"></param>
    public ValueTask<Result> DispatchAsync(ApplyRulesAction action)
    {
        // Don't process if already processing
        if (_processing)
            return ValueTask.FromResult(Result.Failure("Rules already running."));

        _processing = true;

        // apply rules
        if (this.TryNewDateNotInTheFutureRule(out ValidationException? exception))
            return ValueTask.FromResult(Result.Fail(exception!));

        // Notify any listeners that the state has changed
        this.StateHasChanged?.Invoke(action.sender ?? this, this.Id);

        _processing = false;

        return ValueTask.FromResult(Result.Success());
    }

    private bool TryNewDateNotInTheFutureRule([NotNullWhen(true)] out ValidationException? exception)
    {
        exception = null;
        if (_item.State == CommandState.Add && _item.Record.Date.Value > DateOnly.FromDateTime(DateTime.Now))
            return false;

        exception = new ValidationException("A new weather forecast must have a future date.");
        return true;
    }
}
