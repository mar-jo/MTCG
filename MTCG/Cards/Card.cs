using MTCG.Enums;

namespace MTCG.Cards;

public abstract class Card
{
    public abstract string? Name { get; set; }
    public abstract int Damage { get; set; }

}