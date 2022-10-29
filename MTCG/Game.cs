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
        } while (username == null || password == null);

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
        } while (input?.ToLower() != "p" || input.ToLower() != "c");

        if (input.ToLower() == "p")
        {
            player.PurchaseCards();
        }

    }

    public void InitializeDecks()
    {
        Console.WriteLine("Random ListOfCards of Cards for both Players is being generated...\n");
        PlayerOne.CreateDeckOfCards();
        PlayerTwo.CreateDeckOfCards();
    }
}