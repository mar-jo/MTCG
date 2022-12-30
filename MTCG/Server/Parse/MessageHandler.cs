﻿using System.Net;
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

    private static string GetUsernameOutOfToken(Dictionary<string, string> data)
    {
        // in the library, "Authorization" has a string that looks something like "Basic kienboec-mtcgtoken", get the "kienboec" out of it, save it in a string and return it.

        var splitInTwo = data["Authorization"].Split('-')[0];
        var username = splitInTwo.Split(' ')[1];

        Console.WriteLine("[!] Username: " + username);

        return username;
    }

    private static bool IsAuthorized(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "None")
        {
            return false;
        }

        return true;
    }

    private static bool IsAdmin(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "Basic admin-mtcgToken")
        {
            return true;
        }

        return false;
    }

    private static void CreateToken(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "None")
        {
            data["Authorization"] = data["Username"] + "-mtcgToken";
        }
    }

    public static Dictionary<string, string> GetFirstLine(string data)
    {
        var message = new Dictionary<string, string>();
        var lines = data.Split(Environment.NewLine);
        var firstLine = lines[0];
        var firstLineParts = firstLine.Split(' ');

        message.Add("Method", firstLineParts[0]);
        message.Add("Path", firstLineParts[1]);
        message.Add("HTTP", firstLineParts[2]);

        CheckAuthorization(message, data);

        // Debug
        Console.WriteLine($"[!] Method: {message["Method"]}, Path: {message["Path"]}, HTTP: {message["HTTP"]}, Authorization: {message["Authorization"]}.");

        return message;
    }

    public static string BranchHandler(Dictionary<string, string> data, string rest)
    {
        //Dictionary<string, string> combined = new Dictionary<string, string>();

        /*if (data["Path"] == $"/users/{GetUsernameOutOfToken(data)}")
        {

        }*/

        switch (data["Path"])
        {
            case "/users":
                _combined = ParseData.ParseUser(data, rest);
                var httpCode = DBHandler.CreateUser(_combined);
                return ResponseHandler.HttpResponseCodeHandler(httpCode, _combined);
            //case "/sessions":
            //    _combined = ParseData.ParseUser(data, rest);
            //
            //    //TODO: Add handler to check whether login was successful or not
            //    CreateToken(_combined);
            //    break;
            //case "/packages":
            //    if (IsAdmin(data))
            //    {
            //        var fiveCards = ParseData.ParsePackages(data, rest);
            //    }
            //    else
            //    {
            //        Console.WriteLine("[!] User is not authorized.");
            //    }
            //    break;
            //case "/transactions/packages":
            //    if (IsAuthorized(data))
            //    {
            //        DBHandler.AcquirePackage(data);
            //    }
            //    else
            //    {
            //        Console.WriteLine("[!] User is not authorized.");
            //    }
            //    break;
            //case "/cards":
            //    DBHandler.DisplayCards(data);
            //    break;
            //case "/deck":
            //    if (data["Method"] == "GET")
            //    {
            //        DBHandler.DisplayCards(data);
            //    }
            //    else if (data["Method"] == "PUT")
            //    {
            //        var card_ids = ParseData.ParseCard(data, rest);
            //        DBHandler.ConfigureDeck(data, card_ids);
            //    }
            //    break;
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
                HttpResponseMessage code = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return ResponseHandler.HttpResponseCodeHandler(code, data);
        }
    }
}