using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Net;

namespace MTCG.Server.Responses;

public static class Response
{
    public static string FormulateResponseUserCreation(HttpResponseMessage code, Dictionary<string, string> data)
    {
        string headerStuff = $"{data["HTTP"]} {(int)code.StatusCode} {code.StatusCode}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";

        if (code.StatusCode == HttpStatusCode.Conflict)
        {
            body = "[DB] User with same username already registered\n";
            bodyLength = body.Length;

            headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
            headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;
        }
        else
        {
            for (int i = 4; i < data.Count; i++)
            {
                bodyLength += $"[{data.Keys.ElementAt(i).ToUpper()} : {data[data.Keys.ElementAt(i)]}]\n".Length;
            }

            body = "[%] User created successfully!\n";
            bodyLength += body.Length;

            headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
            headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;

            for (int i = 4; i < data.Count; i++)
            {
                body += $"[{data.Keys.ElementAt(i).ToUpper()} : {data[data.Keys.ElementAt(i)]}]\n";
            }
            
        }

        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;

        return response;
    }

    public static string FormulateResponseDefault(HttpResponseMessage code, Dictionary<string, string> data)
    {
        string headerStuff = $"{data["HTTP"]} {(int)code.StatusCode} {code.StatusCode}" + Environment.NewLine;

        int bodyLength = 0;
        string body = "";

        body = "[%] Internal server error.\n";
        bodyLength = body.Length;

        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;

        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;

        return response;
    }

}