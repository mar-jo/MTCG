using MTCG.Enums;

namespace MTCG.Cards;

public class MonsterCard : Card
{
    public Monster Type { get; }

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

    public MonsterCard()
    {
        Random rnd = new Random();
        int num = rnd.Next(0, 5);

        Type = (Monster)num;
        Name = Enum.GetName(typeof(Monster), num);
    }
}