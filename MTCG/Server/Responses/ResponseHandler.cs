using System.Net;

namespace MTCG.Server.Responses;

public class ResponseHandler
{
    public static string HttpResponseCodeHandler(HttpResponseMessage code, Dictionary<string, string> data)
    {
        switch (code.StatusCode)
        {
            case HttpStatusCode.Created:
                return Response.FormulateResponseUserCreation(code, data);
            case HttpStatusCode.Conflict:
                return Response.FormulateResponseUserCreation(code, data);
            default:
                return Response.FormulateResponseDefault(code, data);
        }
    }
}