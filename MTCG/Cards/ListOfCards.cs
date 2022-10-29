namespace MTCG.Cards;

public class ListOfCards
{
    public List<Card> List { get; }

    public ListOfCards()
    {
        List = new List<Card>();
    }

    public void AppendRandomCards()
    {
        Console.WriteLine("5 new Cards have been added to your STACK\n");

        for (int i = 0; i < 5; i++)
        {

        }
    }

    public void printListOfCards()
    {
        foreach(var card in List)
        {
            Console.WriteLine($"- {card}\n");
        }
    }

    public void CreateDeckOfCards(User player)
    {
        if (List.Count < 4)
        {
            throw new Exception("Cannot create ListOfCards if Stack has less than 4 Cards!");
        }

        Random random = new();

        for (int i = 0; i < 5; i++)
        {
            var randomInt = random.Next(0, List.Count - 1);
            //dogshit code
            player.Deck.List.Add(List.ElementAt(randomInt));
            List.RemoveAt(randomInt);
        }
    }
}