namespace MTCG;

public class Game
{
    private User PlayerOne { get; set; }
    private User PlayerTwo { get; set; }
    
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
        PlayerOne.PurchaseOrPlay();

        //Create Second Player
        do
        {
            Console.WriteLine("Player2, input a Username and a Password: ");
            username = Console.ReadLine();
            password = Console.ReadLine();
        } while (username == null || password == null);

        PlayerTwo = new User(username, password);
    }

    public void InitializeDecks()
    {
        Console.WriteLine("Random Deck of Cards for both Players is being generated...\n");
        PlayerOne.CreateDeckOfCards();
        PlayerTwo.CreateDeckOfCards();
    }

    public void PurchaseCards()
    {

    }
}