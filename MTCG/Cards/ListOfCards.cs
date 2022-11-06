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

        Random rnd = new Random();
        int num = rnd.Next(0,1);

        for (int i = 0; i < 5; i++)
        {
            if (num == 0)
            {
                List.Add(new MonsterCard());
            }
            else
            {
                List.Add(new SpellCard());
            }
        }

        Console.WriteLine("Your UPDATED List of Cards:\n");
        PrintListOfCards();
    }

    public void PrintListOfCards()
    {
        if (List.Count == 0)
        {
            Console.WriteLine("Seems like there's nothing in here...\n");
        }
        else
        {
            foreach (var card in List)
            {
                Console.WriteLine($"- {card.Name}\n");
            }
        }
    }

    public void CreateDeckOfCards()
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
            this.List.Add(List.ElementAt(randomInt));
            List.RemoveAt(randomInt);
        }
    }
}