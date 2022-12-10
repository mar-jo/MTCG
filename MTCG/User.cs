using MTCG.Cards;
using System.Numerics;

namespace MTCG;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int Coins { get; set; }
    public ListOfCards Stack { get; }
    public ListOfCards Deck { get; }

    public User(string name, string password)
    {
        this.Username = name;
        this.Password = password;
        this.Coins = 20;
        this.Stack = new ListOfCards();
        this.Deck = new ListOfCards();
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
            this.Coins -= 5;
            Stack.AppendCards();
        }
    }

    public void ResetDeck()
    {
        for (var i = 0; i < 4; i++)
        {
            this.Stack.List.Add(this.Deck.List.Last());
            this.Deck.List.Remove(this.Deck.List.Last());
        }

        Console.WriteLine($"[!] DEBUG : Stack of {this.Username}\n");
        this.Stack.PrintListOfCards();
    }
}