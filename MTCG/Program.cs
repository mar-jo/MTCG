using System;
using MTCG.Database;

namespace MTCG;

class Program
{
    static void Main(string[] args)
    {
        DBHandler.Connect();
        HTTPServer.Server();


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

