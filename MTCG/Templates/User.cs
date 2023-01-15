using MTCG.Cards;
using System.Numerics;
using MTCG.Database;
using MTCG.Enums;
using MTCG.Server.Parse;
using Newtonsoft.Json;

namespace MTCG.Templates;

public class User
{
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("bio")]
    public string Bio { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("coins")]
    public int Coins { get; set; }

    public List<Card> Deck = new();

    public User() { }
    public User(string username, string password, string name, string bio, string image, int coins)
    {
        Username = username;
        Password = password;
        Name = name;
        Bio = bio;
        Image = image;
        Coins = coins;
    }

    public User(Dictionary<string, string> player)
    {
        if (player != null) 
        {
            DBHandler db = new();
            ParseData parser = new();

            Username = parser.GetUsernameOutOfToken(player);
            Deck = db.FetchUserDeck(Deck, player);

            if (Deck.Count == 0)
            {
                Console.WriteLine("[!] Deck is empty!");
                return;
            }

            foreach (var card in Deck)
            {
                if (card.Name.Contains("Fire"))
                    card.Element = Element.Fire;
                else if (card.Name.Contains("Water"))
                    card.Element = Element.Water;
                else if (card.Name.Contains("Regular"))
                    card.Element = Element.Regular;

                if (card.Name.Contains("Goblin"))
                {
                    card.Monster = Monster.Goblin;
                    card.IsMonster = true;
                }
                else if (card.Name.Contains("Dragon"))
                {
                    card.Monster = Monster.Dragon;
                    card.IsMonster = true;
                }
                else if (card.Name.Contains("Wizard"))
                {
                    card.Monster = Monster.Wizard;
                    card.IsMonster = true;
                }
                else if (card.Name.Contains("Ork"))
                {
                    card.Monster = Monster.Ork;
                    card.IsMonster = true;
                }
                else if (card.Name.Contains("Knight"))
                {
                    card.Monster = Monster.Knight;
                    card.IsMonster = true;
                }
                else if (card.Name.Contains("Elve"))
                {
                    card.Monster = Monster.Elve;
                    card.IsMonster = true;
                }

                else if (card.Name.Contains("Spell"))
                    card.IsSpell = true;
            }

            foreach (var card in Deck)
            {
                Console.WriteLine($"[!] CARD : {card.Name}, {card.Damage}, {card.Element}, {card.Monster}, {card.IsSpell}, {card.IsMonster}");
            }
            
        }
    }
}