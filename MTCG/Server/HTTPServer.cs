﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MTCG.Essentials;
using MTCG.Server.Parse;
using MTCG.Server.Responses;

class HTTPServer
{
    private static Lobby _lobby = new();
    static List<string> log = new();

    public static void Server()
    {
        TcpListener? server = null;
        string message = "";
        MessageHandler messageHandler = new();

        try
        {
            Int32 port = 10001;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, port);
            server.Start();

            while (true)
            {
                Console.Write("[%] Listening to incoming Connections... ");
                TcpClient client = server.AcceptTcpClient();

                // IP always the same obviously but it still looks cool...
                IPEndPoint? clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                string clientIP = clientEndPoint.Address.ToString();
                Console.WriteLine($"\n[!] New Client Connected [{clientIP}]");

                Task.Factory.StartNew(() =>
                {
                    Byte[] bytes = new Byte[1024];

                    NetworkStream stream = client.GetStream();
                    int i;
                    Dictionary<string, string> branch = new Dictionary<string, string>();

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine($"\n[!] RECEIVED :\n {data}");

                        branch = messageHandler.GetFirstLine(data);
                        message = messageHandler.BranchHandler(branch, data, _lobby, log);

                        var encodedMsg = System.Text.Encoding.ASCII.GetBytes(message);
                        stream.Write(encodedMsg, 0, encodedMsg.Length);
                    }

                    client.Close();
                });
            }

        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e}");
        }
        finally
        {
            server?.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
}