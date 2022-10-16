using MTCG.Cards;

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
}