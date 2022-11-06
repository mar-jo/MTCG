using MTCG.Enums;

namespace MTCG.Cards;

public class MonsterCard : Card
{
    public Monster MonsterType { get; }
    public Element ElementType { get; }

    public sealed override string? Name
    {
        get => Name;
        set => Name = value;
    }

    public sealed override int Damage
    {
        get => Damage;
        set => Damage = value;
    }

    public MonsterCard()
    {
        Random rnd = new Random();
        int numMonster = rnd.Next(0, 5);
        int numElement = rnd.Next(0, 2);

        MonsterType = (Monster)numMonster;
        ElementType = (Element)numElement;
        Name = Enum.GetName(typeof(Monster), numMonster);

        switch (numElement)
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

        Damage = 30; // TODO: Needs to be changed and fitted to each M-Type
    }
}