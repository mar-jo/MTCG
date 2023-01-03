using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Net;

namespace MTCG.Server.Responses;

//public static class Response
//{
//    public static string FormulateResponseCreation(int statusCode, Dictionary<string, string> data)
//    {
//        string reasonPhrase = reasonPhrases.ContainsKey(statusCode) ? reasonPhrases[statusCode] : "";
//        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;
//
//        int bodyLength = 0;
//        string body = "";
//
//        if (data["Path"].StartsWith("/users") && statusCode == 409)
//        {
//            body = "[] User with same username already registered...\n";
//        }
//        else if (data["Path"].StartsWith("/packages") && statusCode == 409)
//        {
//            body = "[] At least one card in the packages already exists...\n";
//        }
//        else if (data["Path"].StartsWith("/packages"))
//        {
//            body = "[] Package and cards successfully created!\n";
//        }
//        else
//        {
//            body = "[] User created successfully!\n";
//        }
//
//        bodyLength += body.Length;
//
//        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
//        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;
//
//        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;
//
//        return response;
//    }
//
//    public static string FormulateResponseUserData(int statusCode, Dictionary<string, string> data)
//    {
//        string reasonPhrase = reasonPhrases.ContainsKey(statusCode) ? reasonPhrases[statusCode] : "";
//        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;
//
//        int bodyLength = 0;
//        string body = "";
//
//        if (data["Path"].Contains("/sessions") && statusCode == 401)
//        {
//            body = "[] Invalid username/password provided...\n";
//        }
//        else if (data["Path"].Contains("/packages"))
//        {
//            body = "[] Access token is missing or invalid...\n";
//        }
//        else
//        {
//            body = "[] User login successful!\n";
//        }
//
//        bodyLength = body.Length;
//
//        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
//        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;
//
//        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;
//
//        return response;
//    }
//
//
//    public static string FormulateResponseNoAccess(int statusCode, Dictionary<string, string> data)
//    {
//        string reasonPhrase = reasonPhrases.ContainsKey(statusCode) ? reasonPhrases[statusCode] : "";
//        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;
//
//        int bodyLength = 0;
//        string body = "";
//
//        body = "[] Provided user is not \"admin\"...\n";
//        bodyLength = body.Length;
//
//        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
//        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;
//
//        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;
//
//        return response;
//    }
//
//    public static string FormulateResponseDefault(int statusCode, Dictionary<string, string> data)
//    {
//        string reasonPhrase = reasonPhrases.ContainsKey(statusCode) ? reasonPhrases[statusCode] : "";
//        string headerStuff = $"{data["HTTP"]} {statusCode} {reasonPhrase}" + Environment.NewLine;
//
//        int bodyLength = 0;
//        string body = "";
//
//        body = "[] Internal server error.\n";
//        bodyLength = body.Length;
//
//        headerStuff += $"Content-Length: {bodyLength}" + Environment.NewLine;
//        headerStuff += "Content-Type: text/html; charset=utf-8" + Environment.NewLine + "" + Environment.NewLine;
//
//        string response = headerStuff + body + Environment.NewLine + Environment.NewLine;
//
//        return response;
//    }
//
//}