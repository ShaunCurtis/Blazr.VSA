/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

public static class AppDictionary
{
    public static class  Common
    {
        public const string WeatherHttpClient = "WeatherHttpClient";
    }

    public static class WeatherForecast
    {
        public const string WestherForecastId = "Id";
        public const string WestherForecastID = "WestherForecastID";
        public const string Owner = "Owner";
        public const string Date = "Date";
        public const string Temperature = "Temperature";
        public const string Summary = "Summary";

        public const string WeatherForecastAliveAPIUrl = "/api/WeatherForecast/Alive";
        public const string WeatherForecastListAPIUrl = "/api/WeatherForecast/List";
        public const string WeatherForecastRecordAPIUrl = "/api/WeatherForecast/Record";
        public const string WeatherForecastCommandAPIUrl = "/api/WeatherForecast/Command";
    }
}
