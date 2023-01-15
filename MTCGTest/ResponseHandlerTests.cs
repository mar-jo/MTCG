using MTCG.Cards;
using MTCG.Database;
using MTCG.Server.Parse;
using MTCG.Server.Responses;

namespace MTCGTest;
using MTCG;

public class ResponseHandlerTests
{
    private string testData;
    private Dictionary<string, string> testDict;
    private ParseData parser;
    private DBHandler db;
    private MessageHandler messageHandler;
    private ResponseHandler responseHandler;

    [SetUp]
    public void Setup()
    {
        testDict = new Dictionary<string, string>();
        parser = new ParseData();
        db = new DBHandler();
        messageHandler = new MessageHandler();
        responseHandler = new ResponseHandler();
    }

    [Test]
    public void TestBuildingLogForBattleLogic()
    {
        testDict = new Dictionary<string, string>()
        {
            {"HTTP", "HTTP/1.1"}
        };

        List<string> testList = new List<string>()
        {
            "test1",
            "test2",
            "test3",
            ""
        };

        // Arrange
        var expected = "HTTP/1.1 200 OK\r\nContent-Length: 15\r\nContent-Type: text/html; charset=utf-8\r\n\r\ntest1test2test3\r\n\r\n";

        // Act
        var result = responseHandler.BuildLoggingBody(testDict, testList);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestBuildCardResponse()
    {
        // Arrange
        var cards = new[]
        {
            new Card { Id = "random-id-123", Name = "SpookyMonster", Damage = 10 },
            new Card { Id = "random-id-345", Name = "BigChungus", Damage = 20 },
            new Card { Id = "random-id-565", Name = "WhateverMonster", Damage = 30 }
        };

        var body = "";

        // Act
        var result = responseHandler.BuildCardResponse(body, cards);

        // Assert
        var expected = "[] Cards successfully fetched from DB!\n\n" +
                       "{ \"ID\":\"random-id-123\", \"NAME\": \"SpookyMonster\", \"DAMAGE\": \"10\" }\n" +
                       "{ \"ID\":\"random-id-345\", \"NAME\": \"BigChungus\", \"DAMAGE\": \"20\" }\n" +
                       "{ \"ID\":\"random-id-565\", \"NAME\": \"WhateverMonster\", \"DAMAGE\": \"30\" }\n";
        
        Assert.AreEqual(expected, result);
    }

    [Test]
    public void TestBuildScoreBoardResponse()
    {
        List<List<String>> data = new List<List<String>>
        {
            new List<string> { "Player 1", "1000", "5", "2" },
            new List<string> { "Player 2", "1200", "8", "1" },
        };

        // Arrange
        string expectedResponse = "[] The scoreboard could be retrieved successfully!\n" +
                                  "{ \"NAME\": \"Player 1\", \"ELO\": \"1000\", \"WINS\": \"5\", \"LOSSES\": \"2\" }\r\n" +
                                  "{ \"NAME\": \"Player 2\", \"ELO\": \"1200\", \"WINS\": \"8\", \"LOSSES\": \"1\" }";

        string body = "";

        // Act
        string response = responseHandler.BuildScoreBoardResponse(body, data);

        // Assert
        Assert.AreEqual(expectedResponse, response);
    }

    [Test]
    public void TestBuildTradingDataResponse()
    {
        List<List<String>> data = new List<List<String>>
        {
            new List<string> { "1234", "345", "monster", "50", "sdfe45345" },
            new List<string> { "2sdfsgsd", "534frfdsfs", "monkey", "30", "1wer342" },
        };

        // Arrange
        string expectedResponse = "[] There are trading deals available, the response contains these!\n" +
                                  "{ \"TRADEID\": \"1234\", \"TO_TRADE\": \"345\", \"TYPE\": \"monster\", \"MIN_DAMAGE\": \"50\", \"USERID\": \"sdfe45345\" }\r\n" +
                                  "{ \"TRADEID\": \"2sdfsgsd\", \"TO_TRADE\": \"534frfdsfs\", \"TYPE\": \"monkey\", \"MIN_DAMAGE\": \"30\", \"USERID\": \"1wer342\" }";
        
        string body = "";

        // Act
        string response = responseHandler.BuildTradingDataResponse(body, data);

        // Assert
        Assert.AreEqual(expectedResponse, response);
    }

    [Test]
    public void TestRandomRespondCodeToSeeAction()
    {
        testDict = new Dictionary<string, string>()
        {
            {"HTTP", "HTTP/1.1"}
        };

        List<List<string>> testList = new List<List<string>>();

        // Arrange
        var expected = "HTTP/1.1 643 \r\nContent-Length: 26\r\nContent-Type: text/html; charset=utf-8\r\n\r\n[] Internal server error.\n\r\n\r\n";

        // Act
        var result = responseHandler.CreateResponseScoreboard(643, testDict, testList);

        // Assert
        Assert.AreEqual(expected, result);
    }
}