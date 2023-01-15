using MTCG.Database;
using MTCG.Server.Parse;

namespace MTCGTest;
using MTCG;

public class MessageHandlerTests
{
    private string testData;
    private Dictionary<string, string> testDict;
    private ParseData parser;
    private DBHandler db;
    private MessageHandler messageHandler;

    [SetUp]
    public void Setup()
    {
        testDict = new Dictionary<string, string>();
        parser = new ParseData();
        db = new DBHandler();
        messageHandler = new MessageHandler();
    }

    [Test]
    public void TestCheckAuthorization()
    {
        testDict = new Dictionary<string, string>();
        testData = "curl -X GET http://localhost:10001/tradings --header \"Authorization: Abcd sdklfjsldk-mtcgToken\"";

        // Arrange
        string expected = "None";

        // Act
        messageHandler.CheckAuthorization(testDict, testData);

        // Assert
        Assert.AreEqual(testDict["Authorization"], expected);
    }

    [Test]
    public void TestCheckCredibility()
    {
        testDict = new Dictionary<string, string>()
        {
            {"FullPath", "/api/users/test_user/testyboy"},
            {"Authorization", "Aaa testyboy-mtcgToken"}
        };

        // Arrange & Act
        var result = messageHandler.CheckCredibility(testDict);

        // Assert
        Assert.IsTrue(result);
    }
}