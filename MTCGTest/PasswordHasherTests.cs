using MTCG.Cards;
using MTCG.Database;
using MTCG.Database.Hashing;
using MTCG.Server.Parse;
using MTCG.Server.Responses;

namespace MTCGTest;
using MTCG;
using System.Text;

public class PasswordHasherTests
{
    private string testData;
    private Dictionary<string, string> testDict;
    private ParseData parser;
    private DBHandler db;
    private MessageHandler messageHandler;
    private ResponseHandler responseHandler;
    private PasswordHasher passwordHasher;

    [SetUp]
    public void Setup()
    {
        testDict = new Dictionary<string, string>();
        parser = new ParseData();
        db = new DBHandler();
        messageHandler = new MessageHandler();
        responseHandler = new ResponseHandler();
        passwordHasher = new PasswordHasher();
    }

    [Test]
    public void TestHashFunction()
    {
        // Arrange
        string password = "password123";
        
        (string hash, string salt) = passwordHasher.Hash(password);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(hash));
        Assert.IsFalse(string.IsNullOrEmpty(salt));

        // Wollte es auch auf länge testen aber ist schwierig wegen der Implementierung des Algorithmus
    }

    [Test]
    public void TestVerifyFunction_when_hash_and_salt_null()
    {
        string password = "password123";
        string hash = null;
        string salt = null;

        // Call
        bool result = passwordHasher.Verify(password, hash, salt);

        // Assert
        Assert.IsFalse(result);
    }

}