using System.Net;
using MTCG.Server.Parse;

namespace MTCG.Server.Responses;

public class ResponseHandler
{
    private static Dictionary<int, string> reasonPhrases = new Dictionary<int, string>
    {
        { 200, "OK" },
        { 201, "Created" },
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
        string reasonPhrase = reasonPhrases.ContainsKey(statusCode) ? reasonPhrases[statusCode] : "";
        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";
        
        
        switch (statusCode)
        {
            case 200:
            {
                if (data["Path"].StartsWith("/transactions/packages"))
                {
                    body = "[] A package has been successfully bought!";
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
            case 401:
            {
                if (data["Path"].Contains("/sessions"))
                {
                    body = "[] Invalid username/password provided...\n";
                }
                else if (data["Path"].Contains("/packages") || data["Path"].Contains("/transactions"))
                {
                    body = "[] Access token is missing or invalid...\n";
                }

                break;
            }
            case 403:
            {
                if (data["Path"].StartsWith("/transactions/packages"))
                {
                    body = "[] Not enough money for buying a card package...";
                }
                else
                {
                    body = "[] Provided user is not \"admin\"...\n";
                }
                
                break;
            }
            case 404:
            {
                body = "[] No card package available for buying...";

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
}