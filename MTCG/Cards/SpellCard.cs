using MTCG.Enums;

namespace MTCG.Cards;

public class SpellCard : Card
{
    public Element Type { get; }

    public sealed override string? Name
    {
        get { return Name; }
        set { Name = value; }
    }

    public override int Damage
    {
        get { return Damage; }
        set { Damage = value; }
    }

    public SpellCard()
    {
        Random rnd = new Random();
        int num = rnd.Next(0, 5);

        Type = (Element)num;
    }
}
