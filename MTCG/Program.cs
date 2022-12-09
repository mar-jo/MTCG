using System;
namespace MTCG;

class Program
{
    static void Main(string[] args)
    {
        Game game = new();

        Console.WriteLine("Welcome to MONSTER TRADING CARDS GAME\n");
        game.CreatePlayers();

        game.InitializeDecks();

        // TODO: Battle Logic

        game.EndOfGame();
    }
}

