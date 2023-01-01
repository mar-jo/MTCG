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
    
    public static string HttpResponseCodeHandler(HttpResponseMessage code, Dictionary<string, string> data)
    {
        switch (code.StatusCode)
        {
            case HttpStatusCode.Created:
                return Response.FormulateResponseUserCreation(code, data);
            case HttpStatusCode.Conflict:
                return Response.FormulateResponseUserCreation(code, data);
            case HttpStatusCode.OK:
                CreateToken(data);
                return Response.FormulateResponseUserLogin(code, data);
            case HttpStatusCode.Unauthorized:
                return Response.FormulateResponseUserLogin(code, data);
            default:
                return Response.FormulateResponseDefault(code, data);
        }
    }
}