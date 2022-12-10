using System.Collections.Generic;
using System.Reflection.Emit;

namespace MTCG;

public class Game
{
    public User PlayerOne { get; set; } = null!;
    public User PlayerTwo { get; set; } = null!;

    public void CreatePlayers()
    {
        var username = "\0";
        var password = "\0";

        //Create First Player
        do
        {
            Console.WriteLine("Player1, input a Username and a Password: ");
            username = Console.ReadLine();
            password = Console.ReadLine();
        } while (username == null || password == null);

        PlayerOne = new User(username, password);
        PurchaseOrPlay(PlayerOne);

        //Create Second Player
        do
        {
            Console.WriteLine("Player2, input a Username and a Password: ");
            username = Console.ReadLine();
            password = Console.ReadLine();
        } while (username == null && password == null);

        PlayerTwo = new User(username, password);
        PurchaseOrPlay(PlayerTwo);
    }

    public void PurchaseOrPlay(User player)
    {
        var input = "\0";

        do
        {
            Console.WriteLine("Would you like to (P)urchase Cards for your Stack or (C)ontinue the game?");
            input = Console.ReadLine();
        } while (input?.ToLower() != "p" && input.ToLower() != "c");

        if (input.ToLower() == "p")
        {
            player.PurchaseCards();
        }

    }

    public void CreateDeckOfCards(User player)
    {
        if (player.Stack.List.Count < 4)
        {
            new Exception($"Cannot create ListOfCards if Stack has less than 4 Cards : {player.Stack.List.Count} Cards");
        }

        int select = 0;

        Console.WriteLine($"{player.Username}, you can chose 4 cards out of your Stack to use in the battle!\n");

        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine("Cards remaining to chose from:\n");
            player.Stack.PrintListOfCards();

            do
            {
                Console.WriteLine($"Choose a card between 0 and {player.Stack.List.Count - 1}: ");
                select = int.Parse(Console.ReadLine() ?? string.Empty);
            } while (select < 0 || select > player.Stack.List.Count - 1);

            //dogshit code
            player.Deck.List.Add(player.Stack.List.ElementAt(select));
            player.Stack.List.RemoveAt(select);
        }

        Console.WriteLine($"Deck of {player.Username}, locked and loaded!\n");
        player.Deck.PrintListOfCards();
    }

    public void InitializeDecks()
    {
        Console.WriteLine("Random ListOfCards of Cards for both Players is being generated...\n");
        CreateDeckOfCards(PlayerOne);
        CreateDeckOfCards(PlayerTwo);
    }

    public void Battle()
    {

    }

    public void EndOfGame()
    {
        Console.WriteLine("Game Over!");

        PlayerOne.ResetDeck();
        PlayerTwo.ResetDeck();
    }
}