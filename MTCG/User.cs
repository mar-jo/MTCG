namespace MTCG;

public class User
{
    private string Username { get; set; }
    private string Password { get; set; }
    private int Coins { get; set; }
    private List<Card> Stack { get; }
    private List<Card> Deck { get; }

    public User(string name, string password)
    {
        this.Username = name;
        this.Password = password;
        this.Coins = 20;
        this.Stack = new List<Card>();
        this.Deck = new List<Card>();
    }

    public void PurchaseOrPlay()
    {
        var input = "\0";

        do
        {
            Console.WriteLine("Would you like to (P)urchase Cards for your Stack or (C)ontinue the game?");
        } while (input.ToLower() == "p" || input.ToLower() == "c");

        //AUSLAGERN AUF COLLECTION CLASS
    }

    public void CreateDeckOfCards()
    {
        if (Stack.Count < 4)
        {
            throw new Exception("Cannot create Deck if Stack has less than 4 Cards!");
        }

        Random random = new();

        for (int i = 0; i < 5; i++)
        {
            var randomInt = random.Next(0, Stack.Count - 1);

            //dogshit code
            Deck.Add(Stack.ElementAt(randomInt));
            Stack.RemoveAt(randomInt);
        }
    }
}