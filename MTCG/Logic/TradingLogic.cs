namespace MTCG.Logic;

public class TradingLogic
{
    public string CamelCaseSplitter(string? input)
    {
        var output = "";
        var first = true;
        foreach (var c in input)
        {
            if (first)
            {
                output += c;
                first = false;
            }
            else
            {
                if (c.ToString().ToUpper() == c.ToString())
                {
                    output += " " + c;
                }
                else
                {
                    output += c;
                }
            }
        }

        return output;
    }

    public bool ExecuteTrade(string?[] data)
    {
        var type = CamelCaseSplitter(data[2]);
        var minDamage = int.Parse(data[3] ?? string.Empty);

        var selectedCardType = CamelCaseSplitter(data[5]);
        var selectedCardDamage = int.Parse(data[6] ?? string.Empty);

        if (selectedCardType != "Spell")
        {
            selectedCardType = "monster";
        }

        if (type != selectedCardType)
        {
            return false;
        }

        if (minDamage > selectedCardDamage)
        {
            return false;
        }

        return true;
    }
}