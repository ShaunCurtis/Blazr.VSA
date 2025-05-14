/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Shared;

public static class AppDictionary
{
    public static class  Common
    {
        public const string WeatherHttpClient = "WeatherHttpClient";
    }

    public static class Customer
    {
        public const string CustomerId = "Id";
        public const string CustomerID = "CustomerID";
        public const string CustomerName = "CustomerName";
    }

    public static class Invoice
    {
        public const string InvoiceId = "Id";
        public const string InvoiceID = "InvoiceID";
        public const string Date = "Date";
        public const string TotalAmount = "TotalAmount";
    }

    public static class InvoiceItem
    {
        public const string InvoiceItemId = "Id";
        public const string InvoiceItemID = "InvoiceItemID";
        public const string Description = "Description";
        public const string Amount = "Amount";
    }

    public static class WeatherForecast
    {
        public const string WeatherForecastAliveAPIUrl = "/api/WeatherForecast/Alive";
        public const string WeatherForecastListAPIUrl = "/api/WeatherForecast/List";
        public const string WeatherForecastRecordAPIUrl = "/api/WeatherForecast/Record";
        public const string WeatherForecastCommandAPIUrl = "/api/WeatherForecast/Command";
    }
}
