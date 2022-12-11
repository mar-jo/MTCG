using System;
using MTCG.Server;

namespace MTCG;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        HTTPServer server = new HTTPServer(8080);
        server.StartServer();

        /*
        Game game = new();

        Console.WriteLine("Welcome to MONSTER TRADING CARDS GAME\n");

        game.CreatePlayers();
        game.InitializeDecks();
        game.Battle();
        game.EndOfGame();
        */
    }
}

