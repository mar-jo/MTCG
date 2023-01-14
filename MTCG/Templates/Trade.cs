using Newtonsoft.Json;

namespace MTCG.Cards;

public class Trade
{
    [JsonProperty("Id")]
    public string? Id { get; set; }
    [JsonProperty("CardToTrade")]
    public string? CardToTrade { get; set; }
    [JsonProperty("Type")]
    public string? Type { get; set; }
    [JsonProperty("MinimumDamage")]
    public double MinimumDamage { get; set; }
    [JsonProperty("UserId")]
    public string? UserId { get; set; }
    public Trade() { }
}