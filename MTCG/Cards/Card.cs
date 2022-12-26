using MTCG.Enums;
using Newtonsoft.Json;

namespace MTCG.Cards;

public class Card
{
    [JsonProperty("Id")]
    public string? Id { get; set; }
    [JsonProperty("Name")]
    public string? Name { get; set; }
    [JsonProperty("Damage")]
    public double Damage { get; set; }

    public Card() { }

}