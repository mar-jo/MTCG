using MTCG.Enums;

namespace MTCG.Cards;

public class SpellCard : Card
{
    public Element Type { get; }

    public sealed override string? Name { get; set; }

    public sealed override int Damage { get; set; }

    public SpellCard()
    {
        Random rnd = new Random();
        int num = rnd.Next(0, 2);

        Type = (Element)num;
        string[] names = Enum.GetNames(typeof(Element));
        Name = names[num];

        switch (num)
        {
            case 0:
                Damage = 10;
                break;
            case 1:
                Damage = 15;
                break;
            case 2:
                Damage = 20;
                break;
            default:
                throw new Exception("An error has occured during Damage Initialization.");
                break;
        }
    }
}
