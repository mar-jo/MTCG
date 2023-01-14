using MTCG.Cards;
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
        else
        {
            fighterTwo.Deck.Remove(cardFighterTwo);
            fighterOne.Deck.Add(cardFighterTwo);
        }
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

        return ("", 0, false);
    }

    public (string, bool) CheckMonsterFight(Card cardFigherOne, Card cardFighterTwo)
    {
        if (!cardFigherOne.IsMonster || !cardFighterTwo.IsMonster)
        {
            return ("", false);
        }
        
        
    }

    public (string, bool) CheckSpellFight(Card cardFigherOne, Card cardFighterTwo)
    {
        if (!cardFigherOne.IsSpell || !cardFighterTwo.IsSpell)
        {
            return ("", false);
        }

        
    }

    public string CheckMixedFight(Card cardFigherOne, Card cardFighterTwo)
    {
        
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
            log.Add(logEntry);

            if(isSpecialty)
            {
                MoveCardToWinner(fighterOne, fighterTwo, cardFighterOne, cardFighterTwo, winner);

                rounds++;
                continue;
            }

            (logEntry, bool isMonster) = CheckMonsterFight(cardFighterOne, cardFighterTwo);
            log.Add(logEntry);

            if (isMonster)
            {
                rounds++;
                continue;
            }

            (logEntry, bool isSpell) = CheckSpellFight(cardFighterOne, cardFighterTwo);
            log.Add(logEntry);

            if (isSpell)
            {
                rounds++;
                continue;
            }

            logEntry = CheckMixedFight(cardFighterOne, cardFighterTwo);
            log.Add(logEntry);
            
            rounds++;
        } while (rounds != 100);
        
        return null;
    }
}