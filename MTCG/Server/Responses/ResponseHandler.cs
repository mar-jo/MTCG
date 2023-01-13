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
                        body = "[] The deck has been successfully configured!\n";
                    }
                    else if (data.ContainsKey("FullPath"))
                    {
                        body = "[] User successfully updated!\n";
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
                    if (data.ContainsKey("FullPath"))
                    {
                        body = "[] User not found...\n";
                    }
                    else
                    { 
                        body = "[] No card package available for buying...\n";
                    }
                    
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
        headerStuff += "Content-Type: text/json; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;

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

    public static string CreateResponseUsers(int statusCode, string?[] info, Dictionary<string, string> data)
    {
        string reasonPhrase = _reasonPhrases.ContainsKey(statusCode) ? _reasonPhrases[statusCode] : "";
        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";

        switch (statusCode)
        {
            case 200:
            {
                if (data["Path"].StartsWith("/stats"))
                {
                    body += "[] Data retrieved successfully!\n\n" + $"{{ \"NAME\":\"{info[0]}\", \"ELO\": \"{info[1]}\", \"WINS\": \"{info[2]}\", \"LOSSES\": \"{info[3]}\" }}\n";
                }
                else
                {
                    body += "[] Data retrieved successfully!\n\n" + $"{{ \"NAME\":\"{info[0]}\", \"BIO\": \"{info[1]}\", \"IMAGE\": \"{info[2]}\" }}\n";
                }
                break;
            }
            case 401:
            {
                body = "[] Access token is missing or invalid...\n";
                break;
            }
            case 404:
            {
                body = "[] User not found...\n";
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

    public static string CreateResponseScoreboard(int statusCode, Dictionary<string, string> data, List<List<string>> input)
    {
        string reasonPhrase = _reasonPhrases.ContainsKey(statusCode) ? _reasonPhrases[statusCode] : "";
        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";

        switch (statusCode)
        {
            case 200:
                {
                    body += BuildScoreBoardResponse(body, input);
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

    public static string CreateResponseTrading(int statusCode, Dictionary<string, string> data, List<List<string>> input)
    {
        string reasonPhrase = _reasonPhrases.ContainsKey(statusCode) ? _reasonPhrases[statusCode] : "";
        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";

        switch (statusCode)
        {
            case 200:
            {
                if (data["Method"] == "POST")
                {
                    body += "[] Trading deal successfully created!\n";
                }
                else if (data["Method"] == "DELETE")
                {
                    body = "[] Trading deal successfully deleted...\n";
                }
                else
                {
                    body += BuildTradingDataResponse(body, input);
                }
                
                break;
            }
            case 203:
            {
                body += "[] The request was fine, but there are no trading deals available...\n";
                break;
            }
            case 401:
            {
                body = "[] Access token is missing or invalid...\n";
                break;
            }
            case 403:
            {
                if (data["Method"] == "DELETE")
                {
                    body = "[] The provided deal ID was not found...\n";
                }
                else
                {
                    body = "[] The deal contains a card that is not owned by the user or locked in the deck...\n";
                }
                
                break;
            }
            case 404:
            {
                body = "[] The provided deal ID was not found...\n";
                break;
            }
            case 409:
            {
                body = "[] A deal with this deal ID already exists...\n";
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

    public static string BuildTradingDataResponse(string body, List<List<String>> data)
    {
        body += "[] There are trading deals available, the response contains these!\n";
        for (int i = 0; i < data.Count; i++)
        {
            body += "{ ";
            body += $"\"TRADEID\": \"{data[i][0]}\", \"TO_TRADE\": \"{data[i][1]}\", \"TYPE\": \"{data[i][2]}\", \"MIN_DAMAGE\": \"{data[i][3]}\", \"USERID\": \"{data[i][4]}\"";
            body += " }";
            if (i < data.Count - 1)
            {
                body += Environment.NewLine;
            }
        }

        return body;
    }

    public static string BuildScoreBoardResponse(string body, List<List<String>> data)
    {
        body += "[] The scoreboard could be retrieved successfully!\n";
        for (int i = 0; i < data.Count; i++)
        {
            body += "{ ";
            body += $"\"NAME\": \"{data[i][0]}\", \"ELO\": \"{data[i][1]}\", \"WINS\": \"{data[i][2]}\", \"LOSSES\": \"{data[i][3]}\"";
            body += " }";
            if (i < data.Count - 1)
            {
                body += Environment.NewLine;
            }
        }

        return body;
    }


    public static string BuildCardResponse(string body, Card[] cards)
    {
        body = "[] Cards successfully fetched from DB!\n\n";

        foreach (var card in cards)
        {
            body += $"{{ \"ID\":\"{card.Id}\", \"NAME\": \"{card.Name}\", \"DAMAGE\": \"{card.Damage}\" }}\n";
        }

        return body;
    }
}