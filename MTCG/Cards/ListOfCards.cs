namespace MTCG.Cards;

public class ListOfCards
{
    public List<Card> List { get; }

    public ListOfCards()
    {
        List = new List<Card>();
    }

    public void PurchaseCards()
    {
        var input = '\0';
        Console.WriteLine("Would you like to Purchase (M)onster Cards or (S)pell Cards?");
        input = Console.ReadLine()![0];

        //insert purchase logic here
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