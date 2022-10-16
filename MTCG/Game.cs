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
        var choice = PurchaseOrPlay();

        //Create Second Player
        do
        {
            Console.WriteLine("Player2, input a Username and a Password: ");
            username = Console.ReadLine();
            password = Console.ReadLine();
        } while (username == null || password == null);

        PlayerTwo = new User(username, password);
        choice = PurchaseOrPlay();
    }

    public string PurchaseOrPlay()
    {
        var input = "\0";

        do
        {
            Console.WriteLine("Would you like to (P)urchase Cards for your Stack or (C)ontinue the game?");
        } while (input.ToLower() == "p" || input.ToLower() == "c");

        return input.ToLower();
    }

    public void InitializeDecks()
    {
        Console.WriteLine("Random ListOfCards of Cards for both Players is being generated...\n");
        PlayerOne.CreateDeckOfCards();
        PlayerTwo.CreateDeckOfCards();
    }
}