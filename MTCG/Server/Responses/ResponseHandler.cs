using System.Net;
using MTCG.Server.Parse;

namespace MTCG.Server.Responses;

public class ResponseHandler
{
    private static void CreateToken(Dictionary<string, string> data)
    {
        if (data["Authorization"] == "None")
        {
            data["Authorization"] = "Basic " + data["Username"] + "-mtcgToken";
        }
    }

    public static string HttpResponseCodeHandler(int statusCode, Dictionary<string, string> data)
    {
        switch (statusCode)
        {
            case 201:
                return Response.FormulateResponseCreation(statusCode, data);
            case 409:
                return Response.FormulateResponseCreation(statusCode, data);
            case 200:
                CreateToken(data);
                return Response.FormulateResponseUserData(statusCode, data);
            case 401:
                return Response.FormulateResponseUserData(statusCode, data);
            case 403:
                return Response.FormulateResponseNoAccess(statusCode, data);
            default:
                return Response.FormulateResponseDefault(statusCode, data);
        }
    }

}