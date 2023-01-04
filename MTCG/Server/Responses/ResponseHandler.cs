using System.Net;
using MTCG.Cards;
using MTCG.Server.Parse;

namespace MTCG.Server.Responses;

public class ResponseHandler
{
    private static Dictionary<int, string> _reasonPhrases = new Dictionary<int, string>
    {
        { 200, "OK" },
        { 201, "Created" },
        { 203, "No Content" }, // Changed from 204 to 203 because 204 doesn't display content...
        { 400, "Bad Request" },
        { 401, "Unauthorized" },
        { 403, "Forbidden" },
        { 404, "Not Found" },
        { 409, "Conflict" }
    };
    
    private static void CreateToken(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "None")
        {
            data["Authorization"] = "Basic " + data["Username"] + "-mtcgToken";
        }
    }

    public static string HttpResponseCodeHandler(int statusCode, Dictionary<string, string> data)
    {
        string reasonPhrase = _reasonPhrases.ContainsKey(statusCode) ? _reasonPhrases[statusCode] : "";
        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";
        
        
        switch (statusCode)
        {
            case 200:
                {
                    if (data["Path"].StartsWith("/transactions/packages"))
                    {
                        body = "[] A package has been successfully bought!\n";
                    }
                    else if (data["Path"].StartsWith("/deck"))
                    {
                        body = "[] \t\r\nThe deck has been successfully configured!\n";
                    }
                    else
                    {
                        CreateToken(data);
                        body = "[] User login successful!\n";
                    }

                    break;
                }
            case 201:
                {
                    if (data["Path"].StartsWith("/packages"))
                    {
                        body = "[] Package and cards successfully created!\n";
                    }
                    else
                    {
                        body = "[] User created successfully!\n";
                    }

                    break;
                }
            case 400:
                {
                    body = "[] The provided deck did not include the required amount of cards...\n";
                    
                    break;
                }
            case 401:
                {
                    if (data["Path"].Contains("/sessions"))
                    {
                        body = "[] Invalid username/password provided...\n";
                    }
                    else
                    {
                        body = "[] Access token is missing or invalid...\n";
                    }

                    break;
                }
            case 403:
                {
                    if (data["Path"].StartsWith("/transactions/packages"))
                    {
                        body = "[] Not enough money for buying a card package...\n";
                    }
                    else if (data["Path"].StartsWith("/deck"))
                    {
                        body = "[] At least one of the provided cards does not belong to the user or is not available...\n";
                    }
                    else
                    {
                        body = "[] Provided user is not \"admin\"...\n";
                    }
                    
                    break;
                }
            case 404:
                {
                    body = "[] No card package available for buying...\n";

                    break;
                }
            case 409:
                {
                    if (data["Path"].StartsWith("/users"))
                    {
                        body = "[] User with same username already registered...\n";
                    }
                    else if (data["Path"].StartsWith("/packages"))
                    {
                        body = "[] At least one card in the packages already exists...\n";
                    }
                    else
                    {
                        body = "[] Deck was configured already...\n";
                    }
                    break;
                }
            default:
                {
                    body = "[] Internal server error.\n";
                    break;
                }
        }

        
        bodyLength += body.Length;

        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;

        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;

        return response;
    }

    public static string CreateResponseCards(int statusCode, Card[] cards, Dictionary<string, string> data)
    {
        string reasonPhrase = _reasonPhrases.ContainsKey(statusCode) ? _reasonPhrases[statusCode] : "";
        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";

        switch (statusCode)
        {
            case 200:
                {
                    body += BuildCardResponse(body, cards);
                    break;
                }
            case 203:
                {
                    body = "[] The request was fine, but the user doesn't have any cards...\n";
                    break;
                }
            case 401:
                {
                    body = "[] Access token is missing or invalid...\n";
                    break;
                }
            default:
                {
                    body = "[] Internal server error.\n";
                    break;
                }
        }

        bodyLength += body.Length;

        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;

        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;

        return response;
    }

    public static string BuildCardResponse(string body, Card[] cards)
    {
        body = "[] Cards successfully fetched from DB!\n\n";
        
        foreach (var card in cards)
        {
            body += $"{{ \"id\":\"{card.Id}\", \"name\": \"{card.Name}\", \"damage\": \"{card.Damage}\" }}\n";
        }

        return body;
    }
}