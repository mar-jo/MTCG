using MTCG.Database;
using MTCG.Server.Parse;

namespace MTCGTest;
using MTCG;

public class ParseDataTests
{
    private string testData;
    private Dictionary<string, string> testDict;
    private ParseData parser;
    private DBHandler db;

    [SetUp]
    public void Setup()
    {
        testDict = new Dictionary<string, string>();
        parser = new ParseData();
        db = new DBHandler();
    }

    [Test]
    public void ParseUser_ValidInput_ReturnsExpectedDictionary()
    {
        testData = "{\"Username\":\"testuser\",\"Password\":\"testpassword\"}";

        // Arrange
        var expectedDict = new Dictionary<string, string>
        {
            { "Username", "testuser" },
            { "Password", "testpassword" }
        };
        
        // Act
        var result = parser.ParseUser(testDict, testData);

        // Assert
        Assert.AreEqual(expectedDict, result);
    }


    [Test]
    public void ParseUserData_ValidInput_ReturnsExpectedArray()
    {
        testData = "{\"Username\":\"testuser\",\"Bio\":\"Test bio\",\"Image\":\"x_x\"}";

        // Arrange
        var expectedArray = new string[] { "testuser", "Test bio", "x_x" };

        // Act
        var result = parser.ParseUserData(testData);

        // Assert
        Assert.AreEqual(expectedArray, result);
    }

    [Test]
    public void TestParseCard()
    {
        testData = "[\"card_1\", \"card_2\", \"card_3\", \"card_4\"]";

        // Arrange
        var expectedArray = new string[] { "card_1", "card_2", "card_3", "card_4" };

        // Act
        var result = parser.ParseCard(testData);

        // Assert
        Assert.AreEqual(result, expectedArray);
    }

    [Test]
    public void TestParseTradingDeal()
    {
        testDict = new Dictionary<string, string>()
        {
            { "Authorization", "basic helloiamverycool-token" }
        };
        
        testData = "{ \"Id\": \"xyz-123\", \"CardToTrade\": \"abc-345\", \"Type\": \"dog\", \"MinimumDamage\": \"5\" }";

        // Arrange
        var expectedArray = new string[] { "xyz-123", "abc-345", "dog", "5", "helloiamverycool" };

        // Act
        var result = parser.ParseTradingDeal(testDict, testData);

        // Assert
        Assert.AreEqual(result, expectedArray);
    }

    [Test]
    public void TestParseRequestedTradeId()
    {
        testData = "\"verycooltradeid-1234-abcd-456\"";

        // Arrange
        var result = parser.ParseRequestedTradeId(testData);

        // Act
        var expectedResult = "verycooltradeid-1234-abcd-456";

        // Assert
        Assert.AreEqual(result, expectedResult);
    }

    [Test]
    public void TestParsePackages()
    {
        testData = "[{\"Id\":\"1\",\"Name\":\"Card 1\",\"Damage\":\"5\"},{\"Id\":\"2\",\"Name\":\"Card 2\",\"Damage\":\"10\"},{\"Id\":\"3\",\"Name\":\"Card 3\",\"Damage\":\"15\"}]";

        // Arrange
        var result = parser.ParsePackages(testData);

        // Assert
        Assert.AreEqual(result[0].Id, "1");
        Assert.AreEqual(result[0].Name, "Card 1");
        Assert.AreEqual(result[0].Damage, 5);
        
        Assert.AreEqual(result[1].Id, "2");
        Assert.AreEqual(result[1].Name, "Card 2");
        Assert.AreEqual(result[1].Damage, 10);
        
        Assert.AreEqual(result[2].Id, "3");
        Assert.AreEqual(result[2].Name, "Card 3");
        Assert.AreEqual(result[2].Damage, 15);
    }
}