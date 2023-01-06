using System.Net;
using MTCG.Database;
using MTCG.Server.Responses;

namespace MTCG.Server.Parse;

public static class MessageHandler
{
    private static Dictionary<string, string> _combined = new();
    private static void CheckAuthorization(Dictionary<string, string> data, string content)
    {
        var lines = content.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            if (line.StartsWith("Authorization"))
            {
                var lineParts = line.Split(':');
                data.Add("Authorization", lineParts[1].Trim());
            }
        }

        if (!data.ContainsKey("Authorization"))
        {
            data.Add("Authorization", "None");
        }
    }

    public static bool IsAuthorized(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "None")
        {
            return false;
        }

        return true;
    }

    public static bool IsAdmin(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "Basic admin-mtcgToken")
        {
            return true;
        }

        return false;
    }

    public static bool CheckCredibility(Dictionary<string, string> data)
    {
        var tokenName = ParseData.GetUsernameOutOfToken(data);

        if (data["FullPath"].Contains(tokenName))
        {
            return true;
        }

        return false;
    }

    public static Dictionary<string, string> GetFirstLine(string data)
    {
        var message = new Dictionary<string, string>();
        var lines = data.Split(Environment.NewLine);
        var firstLine = lines[0];
        var firstLineParts = firstLine.Split(' ');

        message.Add("Method", firstLineParts[0]);
        message.Add("Path", firstLineParts[1]);
        
        if (message["Path"].StartsWith("/users/"))
        {
            message["Path"] = "/users/username";
            message.Add("FullPath", firstLineParts[1]);
        }

        message.Add("HTTP", firstLineParts[2]);

        CheckAuthorization(message, data);

        // Debug
        Console.WriteLine($"[!] Method: {message["Method"]}, Path: {message["Path"]}, HTTP: {message["HTTP"]}, Authorization: {message["Authorization"]}.");

        return message;
    }

    public static string BranchHandler(Dictionary<string, string> data, string rest)
    {
        int httpCode;

        switch (data["Path"])
        {
            case "/users":
                _combined = ParseData.ParseUser(data, rest);
                httpCode = DBHandler.CreateUser(_combined);
                
                return ResponseHandler.HttpResponseCodeHandler(httpCode, _combined);
            case "/sessions":
                _combined = ParseData.ParseUser(data, rest);
                httpCode = DBHandler.LoginUser(_combined);

                return ResponseHandler.HttpResponseCodeHandler(httpCode, _combined);
            case "/packages":
                var fiveCards = ParseData.ParsePackages(rest);
                httpCode = DBHandler.CreatePackages(fiveCards, data);

                return ResponseHandler.HttpResponseCodeHandler(httpCode, data);
            case "/transactions/packages":
                httpCode = DBHandler.AcquirePackage(data);

                return ResponseHandler.HttpResponseCodeHandler(httpCode, data);
            case "/cards":
                var (statusCode, cards) = DBHandler.DisplayCards(data);

                return ResponseHandler.CreateResponseCards(statusCode, cards, data);
            case "/deck":
            {
                if (data["Method"] == "GET")
                {
                    var(status, card) =  DBHandler.DisplayCards(data);

                    return ResponseHandler.CreateResponseCards(status, card, data);
                }
                else if (data["Method"] == "PUT")
                {
                    var card_ids = ParseData.ParseCard(rest);
                    httpCode = DBHandler.ConfigureDeck(data, card_ids);

                    return ResponseHandler.HttpResponseCodeHandler(httpCode, data);
                }

                return ResponseHandler.HttpResponseCodeHandler(500, data);
            }
            case "/users/username":
                if (data["Method"] == "GET")
                {
                    var(status, info) = DBHandler.DisplayUser(data);

                    return ResponseHandler.CreateResponseUsers(status, info, data);
                }
                else if (data["Method"] == "PUT")
                {
                    var userData = ParseData.ParseUserData(rest);
                    httpCode = DBHandler.InsertUserData(data, userData);

                    return ResponseHandler.HttpResponseCodeHandler(httpCode, data);
                }

                return ResponseHandler.HttpResponseCodeHandler(500, data);
            //case "/stats":
            //    Console.WriteLine("Stats branch");
            //    break;
            //case "/score":
            //    Console.WriteLine("Score branch");
            //    break;
            //case "/battles":
            //    Console.WriteLine("Battles branch");
            //    break;
            //case "/tradings":
            //    Console.WriteLine("Tradings branch");
            //    break;
            default:
                int code = 500;
                return ResponseHandler.HttpResponseCodeHandler(code, data);
        }
    }
}