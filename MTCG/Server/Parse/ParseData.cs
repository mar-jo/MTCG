using MTCG.Cards;
using MTCG.Templates;
using Newtonsoft.Json;
using System.Drawing;

namespace MTCG.Server.Parse;

public class ParseData
{
    public string GetUsernameOutOfToken(Dictionary<string, string> data)
    {
        var splitInTwo = data["Authorization"].Split('-')[0];
        var username = splitInTwo.Split(' ')[1];

        Console.WriteLine("[!] Username: " + username);

        return username;
    }
    
    public Dictionary<string, string> ParseUser(Dictionary<string, string> dict, string data)
    {
        var lines = data.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            if (line.StartsWith("{"))
            {
                User? user = JsonConvert.DeserializeObject<User>(line);
                dict.Add("Username", user?.Username);
                dict.Add("Password", user?.Password);

                //Console.WriteLine($"[!] Username: {dict["Username"]}, Password: {dict["Password"]}");
            }
        }

        return dict;
    }

    public string[] ParseUserData(string data)
    {
        var lines = data.Split(Environment.NewLine);
        var userData = new string[3];

        foreach (var line in lines)
        {
            if (line.StartsWith("{"))
            {
                User? user = JsonConvert.DeserializeObject<User>(line);
                userData[0] = user?.Username!;
                userData[1] = user?.Bio!;
                userData[2] = user?.Image!;
            }
        }

        Console.WriteLine($"[!] Name: {userData[0]}, Bio: {userData[1]}, Image: {userData[2]}");
        return userData;
    }

    public string[] ParseCard(string data)
    {
        var lines = data.Split(Environment.NewLine);

        var size = 0;
        foreach(var line in lines)
        {
            if (line.StartsWith("["))
            {
                size = line.Split(',').Length;
            }
        }

        var card_ids = new string[size];

        foreach (var line in lines)
        {
            if (line.StartsWith("["))
            {
                dynamic? cards = JsonConvert.DeserializeObject<string[]>(line);
        
                for (int i = 0; i < size; i++)
                {
                    card_ids[i] = cards?[i];
                    Console.WriteLine($"[!] CARD_ID : {cards?[i]}");
                }
            }
        }

        return card_ids;
    }

    public string[] ParseTradingDeal(Dictionary<string, string> user, string data)
    {
        var lines = data.Split(Environment.NewLine);

        var deal = new string?[5];

        foreach (var line in lines)
        {
            if (line.StartsWith("{"))
            {
                Trade? trade = JsonConvert.DeserializeObject<Trade>(line);

                deal[0] = trade?.Id!;
                deal[1] = trade?.CardToTrade!;
                deal[2] = trade?.Type!;
                deal[3] = trade?.MinimumDamage!.ToString();
                deal[4] = GetUsernameOutOfToken(user);
            }
        }

        Console.WriteLine($"DEAL => TRADEID : {deal[0]}, USERID : {deal[4]}, CARDTOTRADE : {deal[1]}, TYPE : {deal[2]}, MINIMUM DAMAGE : {deal[3]}");

        return deal as string[];
    }

    public string ParseRequestedTradeId(string data)
    {
        var lines = data.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            if (line.StartsWith("\""))
            {
                var tradeId = line.Split('"')[1];
                Console.WriteLine($"[!] TRADE_ID : {tradeId}");
                return tradeId;
            }
        }
        
        return "";
    }

    public Card[] ParsePackages(string data)
    {
        var lines = data.Split(Environment.NewLine);
        int cardCount = 1, packageCount = 0;
        Card[]? package = new Card[4];

        Console.WriteLine(Environment.NewLine);

        foreach (var line in lines)
        {
            if (line.StartsWith("["))
            {
                var cards = line.Split("{");

                foreach (var card in cards)
                {
                    if (card.StartsWith("\""))
                    {
                        Console.WriteLine($"[+] CARD {cardCount}");
                        package = JsonConvert.DeserializeObject<Card[]>(line);

                        Console.WriteLine($"[!] ID: {package?[packageCount]?.Id}, Name: {package?[packageCount]?.Name}, Damage: {package?[packageCount]?.Damage}\n");
                        cardCount++;
                        packageCount++;
                    }
                }
            }
        }

        return package;
    }
}