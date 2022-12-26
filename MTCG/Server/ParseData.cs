using MTCG.Cards;
using MTCG.Essentials;
using Newtonsoft.Json;

namespace MTCG.Server;

public static class ParseData
{
    public static Dictionary<string, string> ParseUser(Dictionary<string, string> dict, string data)
    {
        var lines = data.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            if (line.StartsWith("{"))
            {
                dynamic? user = JsonConvert.DeserializeObject<User>(line);
                dict.Add("Username", user?.Username);
                dict.Add("Password", user?.Password);

                //Console.WriteLine($"[!] Username: {dict["Username"]}, Password: {dict["Password"]}");
            }
        }

        return dict;
    }

    public static Card[] ParsePackages(Dictionary<string, string> dict, string data)
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

                        //if (package != null)
                        //{
                        //    var value = package[packageCount]?.Id;
                        //    if (value != null) dict.Add("Id", value);
                        //
                        //    var name = package[packageCount]?.Name;
                        //    if (name != null) dict.Add("Name", name);
                        //
                        //    dict.Add("Damage",
                        //        (package[packageCount]?.Damage).ToString() ?? throw new InvalidOperationException());
                        //}
                        // Console.WriteLine($"[!] ID: {dict["Id"]}, Name: {dict["Name"]}, Damage: {dict["Damage"]}");

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