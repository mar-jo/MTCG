using System.Net;
using MTCG.Database;
using MTCG.Essentials;
using MTCG.Server.Responses;

namespace MTCG.Server.Parse;

public class MessageHandler
{
    private Dictionary<string, string> _combined = new();
    public void CheckAuthorization(Dictionary<string, string> data, string content)
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

    public bool IsAuthorized(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "None")
        {
            return false;
        }

        return true;
    }

    public bool IsAdmin(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "Basic admin-mtcgToken")
        {
            return true;
        }

        return false;
    }

    public bool CheckCredibility(Dictionary<string, string> data)
    {
        ParseData parser = new();

        var tokenName = parser.GetUsernameOutOfToken(data);

        if (data["FullPath"].Contains(tokenName))
        {
            return true;
        }

        return false;
    }

    public string ParseTradeID(Dictionary<string, string> data)
    {
        var fullPathParts = data["FullPath"].Split('/');
        var tradeID = fullPathParts[2];

        return tradeID;
    }

    public Dictionary<string, string> GetFirstLine(string data)
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
        else if (message["Path"].StartsWith("/tradings/"))
        {
            message["Path"] = "/tradings/deal";
            message.Add("FullPath", firstLineParts[1]);
        }

        message.Add("HTTP", firstLineParts[2]);

        CheckAuthorization(message, data);

        // Debug
        Console.WriteLine($"[!] Method: {message["Method"]}, Path: {message["Path"]}, HTTP: {message["HTTP"]}, Authorization: {message["Authorization"]}.");

        return message;
    }

    public string BranchHandler(Dictionary<string, string> data, string rest, Lobby lobby, List<string> log)
    {
        int httpCode;
        DBHandler db = new();
        ParseData parser = new();
        ResponseHandler response = new();

        switch (data["Path"])
        {
            case "/users":
                _combined = parser.ParseUser(data, rest);
                httpCode = db.CreateUser(_combined);
                
                return response.HttpResponseCodeHandler(httpCode, _combined);
            case "/sessions":
                _combined = parser.ParseUser(data, rest);
                httpCode = db.LoginUser(_combined);

                return response.HttpResponseCodeHandler(httpCode, _combined);
            case "/packages":
                var fiveCards = parser.ParsePackages(rest);
                httpCode = db.CreatePackages(fiveCards, data);

                return response.HttpResponseCodeHandler(httpCode, data);
            case "/transactions/packages":
                httpCode = db.AcquirePackage(data);

                return response.HttpResponseCodeHandler(httpCode, data);
            case "/cards":
                var (statusCode, cards) = db.DisplayCards(data);

                return response.CreateResponseCards(statusCode, cards, data);
            case "/deck":
            {
                if (data["Method"] == "GET")
                {
                    var(status, card) = db.DisplayCards(data);

                    return response.CreateResponseCards(status, card, data);
                }
                else if (data["Method"] == "PUT")
                {
                    var card_ids = parser.ParseCard(rest);
                    httpCode = db.ConfigureDeck(data, card_ids);

                    return response.HttpResponseCodeHandler(httpCode, data);
                }

                return response.HttpResponseCodeHandler(500, data);
            }
            case "/users/username":
                if (data["Method"] == "GET")
                {
                    var(status, info) = db.DisplayUser(data);

                    return response.CreateResponseUsers(status, info, data);
                }
                else if (data["Method"] == "PUT")
                {
                    var userData = parser.ParseUserData(rest);
                    httpCode = db.InsertUserData(data, userData);

                    return response.HttpResponseCodeHandler(httpCode, data);
                }

                return response.HttpResponseCodeHandler(500, data);
            
            case "/stats":
                var (http, stats) = db.DisplayStatistics(data);

                return response.CreateResponseUsers(http, stats, data);
            case "/score":
                var (intCode, scores) = db.DisplayScoreboard(data);

                return response.CreateResponseScoreboard(intCode, data, scores);
                break;
            case "/battles":
                if (data["Authorization"] != "None")
                {
                    lobby.AddPlayer(data);

                    while (lobby.CheckReadyPlayers() != 2)
                    {
                        Thread.Sleep(1000);
                    }
                    object _lock = new object();
                    bool _hasExecuted = false;

                    lock (_lock)
                    {
                        if (!_hasExecuted)
                        {
                            _hasExecuted = true;
                            log = lobby.InitiateBattle();
                        }

                        db.ResetDeck(data);
                        return response.BuildLoggingBody(data, log);
                    }
                }
                else
                {
                    return response.HttpResponseCodeHandler(401, data);
                }
            case "/tradings":
                if (data["Method"] == "GET")
                {
                    var (a, trades) = db.CheckDeals(data);

                    return response.CreateResponseTrading(a, data, trades);
                }
                else if (data["Method"] == "POST")
                {
                    var tradingDeal = parser.ParseTradingDeal(data, rest);
                    httpCode = db.CreateTradeDeal(data, tradingDeal);

                    return response.CreateResponseTrading(httpCode, data, null!);
                }

                return response.HttpResponseCodeHandler(500, data);
            case "/tradings/deal":
                if (data["Method"] == "POST")
                {
                    var dealid = ParseTradeID(data);
                    var cardid = parser.ParseRequestedTradeId(rest);

                    httpCode = db.ExecuteTradeDeal(data, dealid, cardid);

                    return response.CreateResponseTrading(httpCode, data, null!);
                }
                else if (data["Method"] == "DELETE")
                {
                    httpCode = db.DeleteTradeDeal(data, ParseTradeID(data));

                    return response.CreateResponseTrading(httpCode, data, null!);
                }
            
                return response.HttpResponseCodeHandler(500, data);
            default:
                int code = 500;
                return response.HttpResponseCodeHandler(code, data);
        }
    }
}