using Newtonsoft.Json;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp.Plugins.OpenMeteo;

public class DailyWeather
{
    public required List<string> Time { get; set; }
    [JsonProperty("weather_code")]
    public required List<int> WeatherCode { get; set; }
    [JsonProperty("apparent_temperature_max")]
    public required List<decimal> ApparentTemperatureMax { get; set; }
    [JsonProperty("apparent_temperature_min")]
    public required List<decimal> ApparentTemperatureMin { get; set; }
    [JsonProperty("precipitation_sum")]
    public required List<decimal> Precipitation { get; set; }
    [JsonProperty("rain_sum")]
    public required List<decimal> Rain { get; set; }
    [JsonProperty("showers_sum")]
    public required List<decimal> Showers { get; set; }
    [JsonProperty("snowfall_sum")]
    public required List<decimal> Snowfall { get; set; }
    [JsonProperty("precipitation_probability_max")]
    public required List<int> PrecipitationChanceMax { get; set; }
}