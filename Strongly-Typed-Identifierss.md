# Strongly Typed Identifiers

A major source of bugs in an application is Id mismatching.  Let's look at an example to demonstrate.

```csharp
public record WeatherForecast
{
	public Guid Id {get; init;}
	//...
}

public record WeatherStation
{
	public Guid Id {get; init;}
	//...
}
```

There's nothing to stop you doing this inadvertently.

```csharp
var forecast = new WeatherForecast();
var station = GetStaton(forecast.Id);
```

The solution is to define each Id as a different type. 


