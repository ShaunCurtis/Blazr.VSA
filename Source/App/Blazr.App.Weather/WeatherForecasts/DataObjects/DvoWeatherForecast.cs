﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Weather.Infrastructure;

public sealed record DvoWeatherForecast
{
    [Key] public Guid WeatherForecastID { get; init; } = Guid.Empty;
    public Guid OwnerID { get; init; } = Guid.Empty;
    public string Owner { get; set; } = "[Not Set]";
    public DateTime Date { get; init; }
    public decimal Temperature { get; set; }
    public string? Summary { get; set; }
}
