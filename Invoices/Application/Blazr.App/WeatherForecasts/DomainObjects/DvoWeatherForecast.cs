﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Infrastructure;

public sealed record DvoWeatherForecast
{
    [Key] public Guid WeatherForecastID { get; init; } = Guid.Empty;
    public DateTime Date { get; init; }
    public decimal Temperature { get; set; }
    public string? Summary { get; set; }
}
