using MTCG.Logic;
using MTCG.Server.Parse;
using MTCG.Templates;

namespace MTCG.Essentials;

public class Lobby
{
    private readonly object _lock = new object();
    private List<User> _listOfPlayers { get; set; } = new();
    private List<string> Log = new();

    public void AddPlayer(Dictionary<string, string> player)
    {
        lock (_lock)
        {
            var user = new User(player);
            _listOfPlayers.Add(user);
        }
    }

    public void ClearList()
    {
        _listOfPlayers.Clear();
    }

    public int CheckReadyPlayers()
    {
        return _listOfPlayers.Count;
    }

    public List<string> InitiateBattle()
    {
        BattleLogic battle = new();

        Thread.Sleep(1000);

        if (CheckReadyPlayers() != 0)
        {
            Log = battle.Battle(_listOfPlayers[0], _listOfPlayers[1]);
        }
        
        ClearList();

        return Log;
    }
}