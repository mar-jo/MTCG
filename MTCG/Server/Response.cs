using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MTCG.Server;

public static class Response
{
    public static string FormulateResponse(Dictionary<string, string> data)
    {
        string statusLine = "HTTP/1.1 200 OK\r\n";
        string headers = "Content-Type: text/html; charset=utf-8\r\n";

        int headersLength = Encoding.ASCII.GetByteCount(statusLine + headers);
        int bodyLength = 0;

        for (int i = 4; i < data.Count; i++)
        {
            bodyLength += Encoding.ASCII.GetByteCount($"-> {data.Keys.ElementAt(i).ToUpper()} : {data[data.Keys.ElementAt(i)]}\n");
        }

        headers += $"Content-Length: {headersLength + bodyLength}\r\n";
        headers += "\r\n";

        string body = "";
        for (int i = 4; i < data.Count; i++)
        {
            body += $"-> {data.Keys.ElementAt(i).ToUpper()} : {data[data.Keys.ElementAt(i)]}\n";
        }

        string response = statusLine + headers + body;

        return response;
    }

}