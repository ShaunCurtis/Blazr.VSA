﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public static class AppDictionary
{
    public static class Common
    {
        public const string WeatherHttpClient = "WeatherHttpClient";
    }

    public static class WeatherForecast
    {
        public const string WeatherForecastId = "WeatherForecastId";
        public const string WeatherForecastID = "WeatherForecastID";
        public const string Date = "Date";
        public const string Temperature = "Temperature";
        public const string TemperatureC = "TemperatureC";
        public const string Summary = "Summary";

        public const string WeatherForecastAliveAPIUrl = "/api/WeatherForecast/Alive";
        public const string WeatherForecastListAPIUrl = "/API/WeatherForecast/GetItems";
        public const string WeatherForecastRecordAPIUrl = "/API/WeatherForecast/GetItem";
        public const string WeatherForecastCommandAPIUrl = "/API/WeatherForecast/Command";
    }
}
