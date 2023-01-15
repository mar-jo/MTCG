using MTCG.Cards;
using MTCG.Database;
using MTCG.Enums;
using MTCG.Templates;

namespace MTCG.Logic;

public class BattleLogic
{
    public (Card, Card) GetRandomCards(User fighterOne, User fighterTwo)
    {
        var randomizer = new Random();
        var randomCardOne = randomizer.Next(0, fighterOne.Deck.Count);
        var randomCardTwo = randomizer.Next(0, fighterTwo.Deck.Count);

        var cardOne = fighterOne.Deck[randomCardOne];
        var cardTwo = fighterTwo.Deck[randomCardTwo];

        return (cardOne, cardTwo);
    }

    public void MoveCardToWinner(User fighterOne, User fighterTwo, Card cardFighterOne, Card cardFighterTwo, int winner)
    {
        if (winner == 1)
        {
            fighterOne.Deck.Remove(cardFighterOne);
            fighterTwo.Deck.Add(cardFighterOne);
        }
        else if (winner == 0)
        {
            fighterTwo.Deck.Remove(cardFighterTwo);
            fighterOne.Deck.Add(cardFighterTwo);
        }
    }

    public (int, int) SetNewDamage(Card cardFighterOne, Card cardFighterTwo)
    {
        if (cardFighterOne.Element.ToString() == "Water" && cardFighterTwo.Element.ToString() == "Fire")
        {
            return ((int)(cardFighterOne.Damage * 2), (int)(cardFighterTwo.Damage / 2));
        }

        if (cardFighterTwo.Element.ToString() == "Fire" && cardFighterOne.Element.ToString() == "Water")
        {
            return ((int)(cardFighterOne.Damage / 2), (int)(cardFighterTwo.Damage * 2));
        }

        if (cardFighterOne.Element.ToString() == "Fire" && cardFighterTwo.Element.ToString() == "Regular")
        {
            return ((int)(cardFighterOne.Damage * 2), (int)(cardFighterTwo.Damage / 2));
        }

        if (cardFighterOne.Element.ToString() == "Regular" && cardFighterTwo.Element.ToString() == "Fire")
        {
            return ((int)(cardFighterOne.Damage / 2), (int)(cardFighterTwo.Damage * 2));
        }

        if (cardFighterOne.Element.ToString() == "Regular" && cardFighterTwo.Element.ToString() == "Water")
        {
            return ((int)(cardFighterOne.Damage * 2), (int)(cardFighterTwo.Damage / 2));
        }

        if (cardFighterOne.Element.ToString() == "Water" && cardFighterTwo.Element.ToString() == "Regular")
        {
            return ((int)(cardFighterOne.Damage / 2), (int)(cardFighterTwo.Damage * 2));
        }

        return ((int)cardFighterOne.Damage, (int)cardFighterTwo.Damage);
    }

    public (string, int, bool) CheckSpecialties(Card cardFighterOne, Card cardFighterTwo)
    {
        if ((cardFighterOne.Monster.ToString() == "Goblin" && cardFighterTwo.Monster.ToString() == "Dragon"))
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} wins\n", 1, true);
        }

        if ((cardFighterOne.Monster.ToString() == "Dragon" && cardFighterTwo.Monster.ToString() == "Goblin"))
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterOne.Name} wins\n", 0, true);
        }

        if ((cardFighterOne.Monster.ToString() == "Ork" && cardFighterTwo.Monster.ToString() == "Wizard"))
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} wins\n", 1, true);
        }

        if ((cardFighterOne.Monster.ToString() == "Wizard" && cardFighterTwo.Monster.ToString() == "Ork"))
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterOne.Name} wins\n", 0, true);
        }

        if ((cardFighterOne.Monster.ToString() == "Knight" && (cardFighterTwo.Element.ToString() == "Water" && cardFighterTwo.IsSpell)))
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} wins\n", 1, true);
        }

        if ((cardFighterOne.Element.ToString() == "Water" && cardFighterOne.IsSpell) && cardFighterTwo.Monster.ToString() == "Knight")
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterOne.Name} wins\n", 0, true);
        }

        if (cardFighterOne.Monster.ToString() == "Kraken" && cardFighterTwo.IsSpell)
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterOne.Name} wins\n", 0, true);
        }

        if (cardFighterOne.IsSpell && cardFighterTwo.Monster.ToString() == "Kraken")
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} wins\n", 1, true);
        }

        if ((cardFighterOne.Element.ToString() == "Fire" && cardFighterOne.Monster.ToString() == "Elve") && cardFighterTwo.Monster.ToString() == "Dragon")
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterOne.Name} wins\n", 0, true);
        }

        if ((cardFighterOne.Monster.ToString() == "Dragon") && (cardFighterTwo.Element.ToString() == "Fire" && cardFighterTwo.Monster.ToString() == "Elve"))
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} wins\n", 1, true);
        }

        return ("", 3, false);
    }

    public (string, int, bool) CheckMonsterFight(Card cardFigherOne, Card cardFighterTwo)
    {
        if (!cardFigherOne.IsMonster || !cardFighterTwo.IsMonster)
        {
            return ("", 3, false);
        }

        if ((cardFigherOne.Damage.Equals(cardFighterTwo.Damage)))
        {
            return ($"{cardFigherOne.Name} VS {cardFighterTwo.Name} => Draw\n", 3, true);
        }

        if (cardFigherOne.Damage > cardFighterTwo.Damage)
        {
            return ($"{cardFigherOne.Name} VS {cardFighterTwo.Name} => {cardFigherOne.Name} wins\n", 0, true);
        }

        if (cardFigherOne.Damage < cardFighterTwo.Damage)
        {
            return ($"{cardFigherOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} wins\n", 1, true);
        }

        return ("", 3, false);
    }

    public (string, int) FightWithAbilities(Card cardFighterOne, Card cardFighterTwo)
    {
        int damageOne = 0, damageTwo = 0;

        if (cardFighterOne.IsSpell || cardFighterTwo.IsSpell)
        {
            (damageOne, damageTwo) = SetNewDamage(cardFighterOne, cardFighterTwo);
        }

        if (damageOne.Equals(damageTwo))
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => Draw\n", 3);
        }

        if ((int)damageOne > (int)damageTwo)
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} => {(int)cardFighterOne.Damage} VS {(int)cardFighterTwo.Damage} => {damageOne} VS {damageTwo} -> {cardFighterOne.Name} wins\n", 0);
        }

        if ((int)damageOne < (int)damageTwo)
        {
            return ($"{cardFighterOne.Name} VS {cardFighterTwo.Name} => {cardFighterTwo.Name} => {(int)cardFighterOne.Damage} VS {(int)cardFighterTwo.Damage} => {damageOne} VS {damageTwo} -> {cardFighterTwo.Name} wins\n", 1);
        }

        return ("", 3);
    }

    public List<string> Battle(User fighterOne, User fighterTwo)
    {
        Console.WriteLine($"[!] {fighterOne.Username} vs {fighterTwo.Username} [!]");
        Console.WriteLine("[!] MAY THE BETTER ONE WIN [!]");
        
        int rounds = 0;
        var log = new List<string>();
        string logEntry = "";
        int winner = 0;

        do
        {
            var (cardFighterOne, cardFighterTwo) = GetRandomCards(fighterOne, fighterTwo);
            
            (logEntry, winner, bool isSpecialty) = CheckSpecialties(cardFighterOne, cardFighterTwo);

            if(isSpecialty)
            {
                log.Add(logEntry);
                MoveCardToWinner(fighterOne, fighterTwo, cardFighterOne, cardFighterTwo, winner);

                rounds++;
                continue;
            }

            (logEntry, winner, bool isMonster) = CheckMonsterFight(cardFighterOne, cardFighterTwo);

            if (isMonster)
            {
                log.Add(logEntry);
                MoveCardToWinner(fighterOne, fighterTwo, cardFighterOne, cardFighterTwo, winner);
                
                rounds++;
                continue;
            }

            (logEntry, winner) = FightWithAbilities(cardFighterOne, cardFighterTwo);

            log.Add(logEntry);
            MoveCardToWinner(fighterOne, fighterTwo, cardFighterOne, cardFighterTwo, winner);
            
            rounds++;
        } while (rounds != 100 && fighterOne.Deck.Count != 0 && fighterTwo.Deck.Count != 0);

        if (fighterOne.Deck.Count > fighterTwo.Deck.Count)
        {
            log.Add($"[!] {fighterOne.Username} WINS [!]\n");

            DBHandler.AdjustStatistics(fighterOne.Username, fighterTwo.Username);
        }
        else if (fighterOne.Deck.Count < fighterTwo.Deck.Count)
        {
            log.Add($"[!] {fighterTwo.Username} WINS [!]\n");

            DBHandler.AdjustStatistics(fighterTwo.Username, fighterOne.Username);
        }
        else
        {
            log.Add($"[!] DRAW [!]\n");
        }

        return log;
    }
}