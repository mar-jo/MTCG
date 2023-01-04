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
            Console.Out.WriteLine("[DB] Connection opened...");
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

    public static int CreateUser(Dictionary<string, string> user)
    {
        ConnectDB();

        var cmd = new NpgsqlCommand("INSERT INTO users (username, password_hash, password_salt) VALUES (@username, @password_hash, @password_salt)", _conn);

        var (hash, salt) = PasswordHasher.Hash(user["Password"]);
        
        cmd.Parameters.AddWithValue("username", user["Username"]);
        cmd.Parameters.AddWithValue("password_hash", hash);
        cmd.Parameters.AddWithValue("password_salt", salt);
        cmd.Prepare();
        
        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("[+] User created successfully!");
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("[-] An error occurred " + ex.Message);
            return 409;
        }
        finally
        {
            CloseDB();
        }

        return 201;
    }

    public static int LoginUser(Dictionary<string, string> user)
    {
        ConnectDB();

        var cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", _conn);
        cmd.Parameters.AddWithValue("username", user["Username"]);
        cmd.Prepare();
        
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
            return 401;
        }
        finally
        {
            CloseDB();
        }

        if (PasswordHasher.Verify(user["Password"], passwordHash, passwordSalt))
        {
            Console.WriteLine("[+] Successfully logged in!");
            return 200;
        }
        else
        {
            Console.WriteLine("[-] Wrong username or password has been provided...");
            return 401;
        }
    }

    public static int CreatePackages(Card[] cards, Dictionary<string, string> data)
    {
        if (!MessageHandler.IsAuthorized(data))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return 401;
        }
        else if (MessageHandler.IsAdmin(data))
        {
            ConnectDB();

            var cmd = new NpgsqlCommand("INSERT INTO cards (cardid, name, damage) VALUES (@cardid, @name, @damage)", _conn);
            foreach (var card in cards)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("cardid", card.Id);
                cmd.Parameters.AddWithValue("name", card.Name);
                cmd.Parameters.AddWithValue("damage", card.Damage);
                cmd.Prepare();

                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[+] Card created successfully!");
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine("[-] An error occurred " + ex.Message);
                    CloseDB();
                    return 409;
                }
            }

            cmd = new NpgsqlCommand("INSERT INTO packages (card1, card2, card3, card4, card5) VALUES (@card1, @card2, @card3, @card4, @card5)", _conn);
            cmd.Parameters.AddWithValue("card1", cards[0].Id);
            cmd.Parameters.AddWithValue("card2", cards[1].Id);
            cmd.Parameters.AddWithValue("card3", cards[2].Id);
            cmd.Parameters.AddWithValue("card4", cards[3].Id);
            cmd.Parameters.AddWithValue("card5", cards[4].Id);
            cmd.Prepare();

            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("[+] Cards added to package successfully!");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("[-] An error occurred " + ex.Message);
                CloseDB();
                return 409;
            }

            CloseDB();
            return 201;
        }
        else
        {
            Console.WriteLine("[-] Not authorized to create cards...");
            return 403;
        }
    }

    public static int AcquirePackage(Dictionary<string, string> user)
    {
        if (!MessageHandler.IsAuthorized(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return 401;
        }
        else
        {
            var username = ParseData.GetUsernameOutOfToken(user);

            ConnectDB();
            var cmd = new NpgsqlCommand("SELECT coins FROM users WHERE username = @username", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();
            var reader = cmd.ExecuteReader();
            int coins = 0;
    
            while (reader.Read())
            {
                coins = (int)reader["coins"];
            }
    
            if (coins < 5)
            {
                Console.WriteLine("[-] User has less than 5 Coins...");
                reader.Close();
                CloseDB();
                return 403;
            }

            reader.Close();

            cmd = new NpgsqlCommand("SELECT * FROM packages LIMIT 1", _conn);
            cmd.Prepare();
            reader = cmd.ExecuteReader();
            int? packageId = null;
            string[] cardIds = new string[5];

            while (reader.Read())
            {
                var packageIdString = reader["packageid"].ToString();
                packageId = int.Parse(packageIdString);

                cardIds[0] = (string)reader["card1"];
                cardIds[1] = (string)reader["card2"];
                cardIds[2] = (string)reader["card3"];
                cardIds[3] = (string)reader["card4"];
                cardIds[4] = (string)reader["card5"];

                break;
            }

            if (packageId == null)
            {
                Console.WriteLine("[-] No packages available...");
                reader.Close();
                CloseDB();
                return 404;
            }
            
            reader.Close();

            cmd = new NpgsqlCommand("UPDATE users SET coins = coins - 5 WHERE username = @username", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM packages WHERE packageid = @packageid", _conn);
            cmd.Parameters.AddWithValue("packageid", packageId);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            foreach (var cardId in cardIds)
            {
                cmd = new NpgsqlCommand("INSERT INTO stack (userid, cardid) VALUES (@username, @cardid)", _conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("cardid", cardId);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("[+] Package acquired successfully!");
            CloseDB();
            return 200;
        }
    }

    public static (int, Card[]) DisplayCards(Dictionary<string, string> user)
    {
        if (!MessageHandler.IsAuthorized(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return (401, null)!;
        }
        else
        {
            var username = ParseData.GetUsernameOutOfToken(user);
            ConnectDB();

            NpgsqlCommand cmd;

            if (user["Path"] == "/deck" && user["Method"] == "GET")
            {
                cmd = new NpgsqlCommand("SELECT COUNT(*) FROM deck WHERE username = @username", _conn);
            }
            else
            {
                cmd = new NpgsqlCommand("SELECT COUNT(*) FROM stack WHERE userid = @username", _conn);
            }

            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            int count = 0;

            while (reader.Read())
            {
                var countString = reader["count"].ToString();
                count = int.Parse(countString);
            }

            reader.Close();

            if (count == 0)
            {
                Console.WriteLine("[-] User has no cards...");
                CloseDB();
                return (203, null)!;
            }

            Card[] cards;
            
            if (user["Path"] == "/deck" && user["Method"] == "GET")
            {
                cards = new Card[4];

                for (int j = 0; j < 4; j++)
                {
                    cards[j] = new Card();
                }
            }
            else
            {
                cards = new Card[count];

                for (int j = 0; j < count; j++)
                {
                    cards[j] = new Card();
                }
            }

            if (user["Path"] == "/deck" && user["Method"] == "GET")
            {
                cmd = new NpgsqlCommand("SELECT * FROM deck WHERE username = @username", _conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();

                reader = cmd.ExecuteReader();

                string[] cardIds = new string[4];

                while (reader.Read())
                {
                    cardIds[0] = (string)reader["card1"];
                    cardIds[1] = (string)reader["card2"];
                    cardIds[2] = (string)reader["card3"];
                    cardIds[3] = (string)reader["card4"];
                }

                reader.Close();

                for (int i = 0; i < 4; i++)
                {
                    cmd = new NpgsqlCommand("SELECT * FROM cards WHERE cardid = @cardid", _conn);
                    cmd.Parameters.AddWithValue("cardid", cardIds[i]);
                    cmd.Prepare();

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cards[i].Id = (string)reader["cardid"];
                        cards[i].Name = (string)reader["name"];
                        cards[i].Damage = (int)reader["damage"];
                    }

                    reader.Close();
                }
            }
            else
            {
                cmd = new NpgsqlCommand("SELECT c.cardid, c.name, c.damage FROM cards AS c INNER JOIN stack AS s ON c.cardid = s.cardid WHERE s.userid = @username", _conn);

                cmd.Parameters.AddWithValue("username", username);
                cmd.Prepare();
                reader = cmd.ExecuteReader();

                int i = 0;
                while (reader.Read())
                {
                    string cardid = reader.GetString(0);
                    string name = reader.GetString(1);
                    int damage = reader.GetInt32(2);

                    cards[i].Id = cardid;
                    cards[i].Name = name;
                    cards[i].Damage = damage;

                    i++;
                }

                reader.Close();
            }

            CloseDB();
            return (200, cards);
        }
    }

    public static int ConfigureDeck(Dictionary<string, string> user, string[] ids)
    {
        if (!MessageHandler.IsAuthorized(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return 401;
        }
        else
        {
            if (ids.Length != 4)
            {
                Console.WriteLine("[-] Invalid number of cards...");
                return 400;
            }

            var username = ParseData.GetUsernameOutOfToken(user);
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM deck WHERE username = @username", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            int count = 0;

            while (reader.Read())
            {
                var countString = reader["count"].ToString();
                count = int.Parse(countString);
            }

            reader.Close();

            if (count == 0)
            {
                try
                {
                    cmd = new NpgsqlCommand(
                        "INSERT INTO deck (username, card1, card2, card3, card4) VALUES (@username, @card1, @card2, @card3, @card4)",
                        _conn);
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("card1", ids[0]);
                    cmd.Parameters.AddWithValue("card2", ids[1]);
                    cmd.Parameters.AddWithValue("card3", ids[2]);
                    cmd.Parameters.AddWithValue("card4", ids[3]);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine("[-] At least one card did not belong to the user : " + ex.Message);
                    CloseDB();
                    return 403;
                }
            }
            else
            {
                Console.WriteLine("[-] Deck already full...");
                CloseDB();
                return 409;
            }

            CloseDB();
            Console.WriteLine("[+] Cards successfully added!");
            return 200;
        }
    }
}
