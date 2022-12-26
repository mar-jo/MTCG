using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Net;

namespace MTCG.Server;

public static class Response
{
    public static string FormulateResponse(Dictionary<string, string> data)
    {
        HttpStatusCode statusCode = HttpStatusCode.OK;

        string headerStuff = $"{data["HTTP"]} {(int)statusCode} {statusCode}" + Environment.NewLine;

        int bodyLength = 0;

        for (int i = 4; i < data.Count; i++)
        {
            bodyLength += $"-> {data.Keys.ElementAt(i).ToUpper()} : {data[data.Keys.ElementAt(i)]}\n".Length;
        }

        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;

        string body = "";
        for (int i = 4; i < data.Count; i++)
        {
            body += $"-> {data.Keys.ElementAt(i).ToUpper()} : {data[data.Keys.ElementAt(i)]}\n";
        }

        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;

        return response;
    }

}