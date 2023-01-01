using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MTCG.Cards;
using MTCG.Database.Hashing;
using MTCG.Server.Parse;
using Npgsql;

namespace MTCG.Database;

public class DBHandler
{
    private const string ConnString = "Host=localhost;Username=postgres;Password=admin;Database=MTCG";
    private static NpgsqlConnection? _conn;
    private static void ConnectDB()
    {
        try
        {
            _conn = new NpgsqlConnection(ConnString);
            Console.Out.WriteLine("[DB] Opening connection...");
            _conn.Open();
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("[DB] An error occurred: " + ex.Message);
        }
    }

    private static void CloseDB()
    {
        try
        {
            _conn.Close();
            Console.Out.WriteLine("[DB] Connection closed...");
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("[DB] An error occurred: " + ex.Message);
        }
    }

    public static HttpResponseMessage CreateUser(Dictionary<string, string> user)
    {
        ConnectDB();

        var cmd = new NpgsqlCommand("INSERT INTO users (username, password_hash, password_salt) VALUES (@username, @password_hash, @password_salt)", _conn);

        var (hash, salt) = PasswordHasher.Hash(user["Password"]);
        
        cmd.Parameters.AddWithValue("username", user["Username"]);
        cmd.Parameters.AddWithValue("password_hash", hash);
        cmd.Parameters.AddWithValue("password_salt", salt);
        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("[+] User created successfully!");
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("[-] An error occurred " + ex.Message);
            return new HttpResponseMessage(HttpStatusCode.Conflict);
        }
        finally
        {
            CloseDB();
        }

        return new HttpResponseMessage(HttpStatusCode.Created);
    }

    public static HttpResponseMessage LoginUser(Dictionary<string, string> user)
    {
        ConnectDB();

        var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", _conn);
        cmd.Parameters.AddWithValue("username", user["Username"]);
        var reader = cmd.ExecuteReader();
        string? passwordHash = "";
        string? passwordSalt = "";

        try
        {
            while (reader.Read())
            {
                passwordHash = reader["password_hash"].ToString();
                passwordSalt = reader["password_salt"].ToString();
            }
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("[-] Wrong username or password has been provided...");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
        finally
        {
            CloseDB();
        }

        if (PasswordHasher.Verify(user["Password"], passwordHash, passwordSalt))
        {
            Console.WriteLine("[+] Successfully logged in!");
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        else
        {
            Console.WriteLine("[-] Wrong username or password has been provided...");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }

    public static HttpResponseMessage CreatePackages(Card[] cards, Dictionary<string, string> data)
    {
        if (MessageHandler.IsAuthorized(data))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
        else if (MessageHandler.IsAdmin(data))
        {
            ConnectDB();

            //cards has 5 cards

            Console.WriteLine("[+] Created package successfully!");
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
        else
        {
            Console.WriteLine("[-] Not authorized...");
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }

    public static void AcquirePackage(Dictionary<string, string> user)
    {
        Console.WriteLine("[%] Not implemented yet!");
    }

    public static void DisplayCards(Dictionary<string, string> user)
    {
        try
        {
            if (user["Authorization"] == "None")
            {
                throw new Exception("[%] Invalid Authorization!");
            }
            else
            {
                Console.WriteLine("[%] Not implemented yet!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Data}");
        }
    }

    public static void ConfigureDeck(Dictionary<string, string> user, string[] ids)
    {
        try
        {
            if (user["Authorization"] == "None")
            {
                throw new Exception("[%] Invalid Authorization!");
            }
            else
            {
                Console.WriteLine("[%] Not implemented yet!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Data}");
        }
    }
}
