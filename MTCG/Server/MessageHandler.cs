namespace MTCG.Server;

public static class MessageHandler
{
    private static Dictionary<string, string> CheckAuthorization(Dictionary<string, string> data, string content)
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

        return data;
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

    public static void BranchHandler(Dictionary<string, string> data, string rest)
    {
        Dictionary<string, string> combined = new Dictionary<string, string>();

        switch (data["Path"])
        {
            case "/users":
                combined = ParseData.ParseUser(data, rest);
                break;
            case "/sessions":
                combined = ParseData.ParseUser(data, rest);
                
                //TODO: Add handler to check whether login was successful or not
                CreateToken(combined);
                break;
            case "/packages":
                var fiveCards = ParseData.ParsePackages(data, rest);
                break;
            case "/transactions/packages":
                Console.WriteLine("Transactions branch");
                break;
            default:
                throw new Exception("Invalid branch");
        }
    }
}