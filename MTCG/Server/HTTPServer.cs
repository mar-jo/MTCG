using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Server;
public class HTTPServer
{
    public const String MSG_DIR = "root/msg/";
    public const String WEB_DIR = "root/web/";

    public const String VERSION = "HTTP/1.1";
    public const String NAME = "[MTCG] - HTTP Server";

    private bool _connected = false;
    private TcpListener _listener;

    public HTTPServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public void StartServer()
    {
        Thread serverThread = new Thread(new ThreadStart(Run));
        serverThread.Start();
    }

    public void Run()
    {
        _connected = true;
        _listener.Start();

        while (_connected)
        {
            Console.WriteLine("Waiting for connection...");

            TcpClient client = _listener.AcceptTcpClient();
            Console.WriteLine("Connection Successful!");

            HandleClient(client);
            client.Close();
        }

        _connected = false;
        _listener.Stop();
    }

    private void HandleClient(TcpClient client)
    {
        StreamReader reader = new StreamReader(client.GetStream());

        String message = "";

        while (reader.Peek() != -1)
        {
            message += reader.ReadLine() + "\n";
        }

        Debug.WriteLine("[!] REQUEST :\n" + message);

        Request request = Request.GetRequest(message);
        Response response = Response.From(request);
        response.Post(client.GetStream());
    }
}
