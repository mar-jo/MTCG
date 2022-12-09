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
        int num = 0;

        for (int i = 0; i < 5; i++)
        {
            num = rnd.Next(0, 2);

            Console.WriteLine(num);

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
}