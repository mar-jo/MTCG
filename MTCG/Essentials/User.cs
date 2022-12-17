using MTCG.Cards;
using System.Numerics;

namespace MTCG.Essentials;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int Coins { get; set; }
    public ListOfCards Stack { get; }
    public ListOfCards Deck { get; }

    public User(string name, string password)
    {
        Username = name;
        Password = password;
        Coins = 20;
        Stack = new ListOfCards();
        Deck = new ListOfCards();
    }

    public void PurchaseCards()
    {
        var input = "\0";

        Console.WriteLine("You have following cards in your STACK\n");
        Stack.PrintListOfCards();

        do
        {
            Console.WriteLine("Would you like to purchase a Package of 5 Cards [y/n]");
            input = Console.ReadLine();
        } while (input?.ToLower() is not "y" or "n");

        if (input.ToLower() == "y")
        {
            Coins -= 5;
            Stack.AppendCards();
        }
    }

    public void ResetDeck()
    {
        for (var i = 0; i < 4; i++)
        {
            Stack.List.Add(Deck.List.Last());
            Deck.List.Remove(Deck.List.Last());
        }

        Console.WriteLine($"[!] DEBUG : Stack of {Username}\n");
        Stack.PrintListOfCards();
    }
}