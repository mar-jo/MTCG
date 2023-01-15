using MTCG.Cards;
using MTCG.Database;
using MTCG.Database.Hashing;
using MTCG.Enums;
using MTCG.Logic;
using MTCG.Server.Parse;
using MTCG.Server.Responses;

namespace MTCGTest;
using MTCG;
using MTCG.Templates;
using System.Text;


public class BattleLogicTests
{
    private string testData;
    private Dictionary<string, string> testDict;
    private ParseData parser;
    private DBHandler db;
    private MessageHandler messageHandler;
    private ResponseHandler responseHandler;
    private PasswordHasher passwordHasher;
    private BattleLogic battleLogic;
    private User testUser1;
    private User testUser2;

    [SetUp]
    public void Setup()
    {
        testDict = new Dictionary<string, string>();
        parser = new ParseData();
        db = new DBHandler();
        messageHandler = new MessageHandler();
        responseHandler = new ResponseHandler();
        passwordHasher = new PasswordHasher();
        battleLogic = new BattleLogic();
        testUser1 = new User();
        testUser2 = new User();
    }

    [Test]
    public void TestBattleLogicMoveCardToWinner()
    {
        testUser1.Deck.Add(new Card { Id = "random-id-123", Name = "card1", Damage = 10 });
        testUser1.Deck.Add(new Card { Id = "random-id-124", Name = "card2", Damage = 10 });
        testUser1.Deck.Add(new Card { Id = "random-id-125", Name = "card3", Damage = 10 });

        testUser2.Deck.Add(new Card { Id = "random-id-126", Name = "card4", Damage = 10 });
        testUser2.Deck.Add(new Card { Id = "random-id-127", Name = "card5", Damage = 10 });
        testUser2.Deck.Add(new Card { Id = "random-id-128", Name = "card6", Damage = 10 });

        // Arrange
        battleLogic.MoveCardToWinner(testUser1, testUser2, testUser1.Deck[0], testUser2.Deck[0], 0);

        // Act
        var result = testUser1.Deck.Count;

        // Assert
        Assert.AreEqual(4, result);
    }

    [Test]
    public void TestBattleLogicMoveCardToWinnerButTheOtherWay()
    {
        testUser1.Deck.Add(new Card { Id = "random-id-123", Name = "card1", Damage = 10 });
        testUser1.Deck.Add(new Card { Id = "random-id-124", Name = "card2", Damage = 10 });
        testUser1.Deck.Add(new Card { Id = "random-id-125", Name = "card3", Damage = 10 });

        testUser2.Deck.Add(new Card { Id = "random-id-126", Name = "card4", Damage = 10 });
        testUser2.Deck.Add(new Card { Id = "random-id-127", Name = "card5", Damage = 10 });
        testUser2.Deck.Add(new Card { Id = "random-id-128", Name = "card6", Damage = 10 });

        // Arrange
        battleLogic.MoveCardToWinner(testUser1, testUser2, testUser1.Deck[0], testUser2.Deck[0], 1);

        // Act
        var result = testUser2.Deck.Count;

        // Assert
        Assert.AreEqual(4, result);
    }

    [Test]
    public void TestSpecialAbilities()
    {
        testUser1.Deck.Add(new Card { Id = "random-id-123", Name = "card1", Damage = 10 });
        testUser2.Deck.Add(new Card { Id = "random-id-126", Name = "card4", Damage = 10 });

        // Arrange
        var(damageone, damagetwo) = battleLogic.SetNewDamage(testUser1.Deck[0], testUser2.Deck[0]);

        // Assert
        Assert.AreEqual(10, damageone);
        Assert.AreEqual(10, damagetwo);
    }

    [Test]
    public void TestCheckMonsterFight()
    {
        testUser1.Deck.Add(new Card { Id = "random-id-123", Name = "card1", Damage = 10, Element = Element.None, Monster = Monster.Goblin, IsSpell = false, IsMonster = true });
        testUser2.Deck.Add(new Card { Id = "random-id-126", Name = "card4", Damage = 10, Element = Element.None, Monster = Monster.Goblin, IsSpell = false, IsMonster = true });

        // Arrange
        var (log, winner, isMonsterFight) = battleLogic.CheckMonsterFight(testUser1.Deck[0], testUser2.Deck[0]);

        // Assert
        Assert.AreEqual(true, isMonsterFight);
        Assert.AreEqual(3, winner);
    }

    [Test]
    public void CheckFightWithAbilities()
    {
        testUser1.Deck.Add(new Card { Id = "random-id-123", Name = "card1", Damage = 10, Element = Element.None, Monster = Monster.Goblin, IsSpell = false, IsMonster = true });
        testUser2.Deck.Add(new Card { Id = "random-id-126", Name = "card4", Damage = 10, Element = Element.None, Monster = Monster.Goblin, IsSpell = false, IsMonster = true });

        // Arrange
        var (log, winner) = battleLogic.FightWithAbilities(testUser1.Deck[0], testUser2.Deck[0]);

        // Assert
        Assert.AreEqual(3, winner);
    }

}