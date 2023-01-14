using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MTCG.Cards;
using MTCG.Database.Hashing;
using MTCG.Logic;
using MTCG.Server.Parse;
using MTCG.Templates;
using Npgsql;
using NpgsqlTypes;

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

        var cmd = new NpgsqlCommand("INSERT INTO users (username, name, password_hash, password_salt) VALUES (@username, @username, @password_hash, @password_salt)", _conn);

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

        cmd = new NpgsqlCommand("INSERT INTO statistics (userid) VALUES (@username)", _conn);
        cmd.Parameters.AddWithValue("username", user["Username"]);
        cmd.Prepare();

        try
        {
            cmd.ExecuteNonQuery();
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

    public static (int, string[]) DisplayUser(Dictionary<string, string> user)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return (401, null!);
        }
        else if (!MessageHandler.CheckCredibility(user))
        {
            Console.WriteLine("[-] Invalid path...");
            return (401, null!);
        }
        else
        {
            var username = ParseData.GetUsernameOutOfToken(user);
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", _conn);
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
                Console.WriteLine("[-] User does not exist...");
                CloseDB();
                return (404, null!);
            }

            cmd = new NpgsqlCommand("SELECT * FROM users WHERE username = @username", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();

            reader = cmd.ExecuteReader();
            
            var result = new string[3];
            while (reader.Read())
            {
                result[0] = reader["name"] is DBNull ? "NULL" : (string)reader["name"];
                result[1] = reader["bio"] is DBNull ? "NULL" : (string)reader["bio"];
                result[2] = reader["image"] is DBNull ? "NULL" : (string)reader["image"];
            }

            reader.Close();
            CloseDB();
            Console.WriteLine("[+] Data retrieved!");
            return (200, result);
        }
    }

    public static int InsertUserData(Dictionary<string, string> user, string[] data)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return 401;
        }
        else if (!MessageHandler.CheckCredibility(user))
        {
            Console.WriteLine("[-] Invalid path...");
            return 401;
        }
        else
        {
            var username = ParseData.GetUsernameOutOfToken(user);
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username", _conn);
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
                Console.WriteLine("[-] User does not exist...");
                CloseDB();
                return 404;
            }

            try
            {
                cmd = new NpgsqlCommand("UPDATE users SET bio = @bio, image = @image, name = @name WHERE username = @username", _conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("name", data[0]);
                cmd.Parameters.AddWithValue("bio", data[1]);
                cmd.Parameters.AddWithValue("image", data[2]);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }

            CloseDB();
            Console.WriteLine("[+] Data updated!");
            return 200;
        }
    }

    public static (int, string?[]) DisplayStatistics(Dictionary<string, string> user)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return (401, null!);
        }
        else
        {
            var username = ParseData.GetUsernameOutOfToken(user);
            
            ConnectDB();
            var cmd = new NpgsqlCommand("SELECT name FROM users WHERE username = @username", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            var result = new string?[4];

            while (reader.Read())
            {
                result[0] = reader["name"].ToString();
            }

            reader.Close();

            cmd = new NpgsqlCommand("SELECT elo, wins, losses FROM statistics WHERE userid = @username", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                result[1] = reader["elo"].ToString();
                result[2] = reader["wins"].ToString();
                result[3] = reader["losses"].ToString();
            }

            reader.Close();
            CloseDB();
            Console.WriteLine("[+] Data retrieved successfully!");
            return (200, result);
        }
    }

    public static (int, List<List<string?>>) DisplayScoreboard(Dictionary<string, string> user)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return (401, null!);
        }
        else
        {
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT users.name, statistics.elo, statistics.wins, statistics.losses FROM statistics JOIN users ON statistics.userid = users.username ORDER BY statistics.elo", _conn);

            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            var scoreBoard = new List<List<string?>>();

            while (reader.Read())
            {
                var temp = new List<string?>
                {
                    reader["name"].ToString(),
                    reader["elo"].ToString(),
                    reader["wins"].ToString(),
                    reader["losses"].ToString()
                };
                scoreBoard.Add(temp);
            }

            reader.Close();
            CloseDB();
            Console.WriteLine("[+] Data retrieved successfully!");
            return (200, scoreBoard);
        }
    }

    public static (int, List<List<string?>>) CheckDeals(Dictionary<string, string> user)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return (401, null!);
        }
        else
        {
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM trading", _conn);
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            int count = 0;

            while (reader.Read())
            {
                var countString = reader["count"].ToString();
                count = int.Parse(countString);
            }

            reader.Close();
            CloseDB();

            if (count == 0)
            {
                Console.WriteLine("[-] No deals found...");
                return (203, null!);
            }

            var trades = new List<List<string?>>();
            ConnectDB();
            cmd = new NpgsqlCommand("SELECT * FROM trading", _conn);
            cmd.Prepare();

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var temp = new List<string?>
                {
                    reader["tradeid"].ToString(),
                    reader["to_trade"].ToString(),
                    reader["type"].ToString(),
                    reader["min_damage"].ToString(),
                    reader["userid"].ToString()
                };
                trades.Add(temp);
            }

            reader.Close();
            CloseDB();

            Console.WriteLine("[+] Data retrieved successfully!");
            return (200, trades);
        }
    }

    public static int CreateTradeDeal(Dictionary<String, String> user, string[] deal)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return 401;
        }
        else
        {
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM trading WHERE tradeid = @tradeid", _conn);
            cmd.Parameters.AddWithValue("tradeid", deal[0]);
            cmd.Prepare();

            var reader = cmd.ExecuteReader();
            int count = 0;

            while (reader.Read())
            {
                var countString = reader["count"].ToString();
                count = int.Parse(countString);
            }

            reader.Close();

            if (count != 0)
            {
                Console.WriteLine("[-] Trade deal already exists...");
                CloseDB();
                return 409;
            }

            var username = ParseData.GetUsernameOutOfToken(user);
            cmd = new NpgsqlCommand("SELECT COUNT(*) FROM stack WHERE userid = @username AND cardid = @cardid", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("cardid", deal[1]);
            cmd.Prepare();

            reader = cmd.ExecuteReader();
            count = 0;

            while (reader.Read())
            {
                var countString = reader["count"].ToString();
                count = int.Parse(countString);
            }

            reader.Close();

            if (count == 0)
            {
                Console.WriteLine("[-] User does not have the item in their stack...");
                CloseDB();
                return 403;
            }

            cmd = new NpgsqlCommand("INSERT INTO trading (tradeid, userid, to_trade, type, min_damage) VALUES (@tradeid, @userid, @to_trade, @type, @min_damage)", _conn);
            cmd.Parameters.AddWithValue("tradeid", deal[0]);
            cmd.Parameters.AddWithValue("userid", username);
            cmd.Parameters.AddWithValue("to_trade", deal[1]);
            cmd.Parameters.AddWithValue("type", deal[2]);
            cmd.Parameters.AddWithValue("min_damage", Int32.Parse(deal[3]));
            cmd.Prepare();

            cmd.ExecuteNonQuery();
            CloseDB();
            Console.WriteLine("[+] Trade deal created successfully!");
            return 200;
        }
    }

    public static int DeleteTradeDeal(Dictionary<String, String> user, String tradeid)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return 401;
        }
        else
        {
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM trading WHERE tradeid = @tradeid", _conn);
            cmd.Parameters.AddWithValue("tradeid", tradeid);
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
                Console.WriteLine("[-] Trade deal does not exist...");
                CloseDB();
                return 404;
            }

            var username = ParseData.GetUsernameOutOfToken(user);
            cmd = new NpgsqlCommand("SELECT COUNT(*) FROM trading WHERE tradeid = @tradeid AND userid = @username", _conn);
            cmd.Parameters.AddWithValue("tradeid", tradeid);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();

            reader = cmd.ExecuteReader();
            count = 0;

            while (reader.Read())
            {
                var countString = reader["count"].ToString();
                count = int.Parse(countString);
            }

            reader.Close();

            if (count == 0)
            {
                Console.WriteLine("[-] User is not the owner of the deal...");
                CloseDB();
                return 403;
            }

            cmd = new NpgsqlCommand("DELETE FROM trading WHERE tradeid = @tradeid", _conn);
            cmd.Parameters.AddWithValue("tradeid", tradeid);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
            CloseDB();
            Console.WriteLine("[+] Trade deal deleted successfully!");
            return 200;
        }
    }

    public static int ExecuteTradeDeal(Dictionary<string, string> user, string? dealid, string? cardid)
    {
        if (!MessageHandler.IsAuthorized(user) && !MessageHandler.IsAdmin(user))
        {
            Console.WriteLine("[-] Invalid access-token...");
            return 401;
        }
        else
        {
            ConnectDB();

            var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM trading WHERE tradeid = @tradeid", _conn);
            cmd.Parameters.AddWithValue("tradeid", dealid);
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
                Console.WriteLine("[-] Trade deal does not exist...");
                CloseDB();
                return 404;
            }

            var username = ParseData.GetUsernameOutOfToken(user);
            cmd = new NpgsqlCommand("SELECT COUNT(*) FROM stack WHERE userid = @username AND cardid = @cardid", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("cardid", cardid);
            cmd.Prepare();

            reader = cmd.ExecuteReader();
            count = 0;

            while (reader.Read())
            {
                var countString = reader["count"].ToString();
                count = int.Parse(countString);
            }

            reader.Close();

            if (count == 0)
            {
                Console.WriteLine("[-] User does not have the item in their stack...");
                CloseDB();
                return 403;
            }

            cmd = new NpgsqlCommand("SELECT * FROM trading WHERE tradeid = @tradeid", _conn);
            cmd.Parameters.AddWithValue("tradeid", dealid);
            cmd.Prepare();

            reader = cmd.ExecuteReader();
            string? uid = null;

            while (reader.Read())
            {
                uid = reader["userid"].ToString();
            }

            reader.Close();

            if (uid == ParseData.GetUsernameOutOfToken(user))
            {
                Console.WriteLine("[-] Can't trade with yourself...");
                CloseDB();
                return 403;
            }

            cmd = new NpgsqlCommand("SELECT * FROM trading WHERE tradeid = @tradeid", _conn);
            cmd.Parameters.AddWithValue("tradeid", dealid);
            cmd.Prepare();

            reader = cmd.ExecuteReader();
            var deal = new string?[7];

            while (reader.Read())
            {
                deal[0] = reader["tradeid"].ToString();
                deal[1] = reader["to_trade"].ToString();
                deal[2] = reader["type"].ToString();
                deal[3] = reader["min_damage"].ToString();
                deal[4] = reader["userid"].ToString();
            }

            reader.Close();

            cmd = new NpgsqlCommand("SELECT * FROM cards WHERE cardid = @cardid", _conn);
            cmd.Parameters.AddWithValue("cardid", cardid);
            cmd.Prepare();

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                deal[5] = reader["name"].ToString();
                deal[6] = reader["damage"].ToString();
            }
            
            reader.Close();
            CloseDB();
            
            var initTrade = new TradingLogic();
            var tradeDecision = initTrade.ExecuteTrade(deal);

            if (!tradeDecision)
            {
                Console.WriteLine("[-] Offered card does not meet the requirements...");
                return 403;
            }

            ConnectDB();
            cmd = new NpgsqlCommand("UPDATE stack SET userid = @username WHERE userid = @uid AND cardid = @cardid", _conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("uid", uid);
            cmd.Parameters.AddWithValue("cardid", deal[1]);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("UPDATE stack SET userid = @uid WHERE userid = @username AND cardid = @cardid", _conn);
            cmd.Parameters.AddWithValue("uid", uid);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("cardid", cardid);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("DELETE FROM trading WHERE tradeid = @tradeid", _conn);
            cmd.Parameters.AddWithValue("tradeid", dealid);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
            CloseDB();
            Console.WriteLine("[+] Trade deal executed successfully!");
            return 200;
        }
    }

    public static List<Card> FetchUserDeck(List<Card> cards, Dictionary<string, string> player)
    {
        ConnectDB();
        var cmd = new NpgsqlCommand("SELECT deck.username, cards.* FROM deck, cards WHERE username = @username", _conn);
        cmd.Parameters.AddWithValue("username", ParseData.GetUsernameOutOfToken(player));
        cmd.Prepare();

        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var card = new Card();
            card.Id = reader["cardid"].ToString();
            card.Name = reader["name"].ToString();
            card.Damage = int.Parse(reader["damage"].ToString());
            cards.Add(card);
        }

        reader.Close();
        CloseDB();
        return cards;
    }

    public static void ResetDeck(Dictionary<string, string> player)
    {
        ConnectDB();
        var cmd = new NpgsqlCommand("DELETE FROM deck WHERE username = @username", _conn);
        cmd.Parameters.AddWithValue("username", ParseData.GetUsernameOutOfToken(player));
        cmd.Prepare();

        cmd.ExecuteNonQuery();
        CloseDB();
        Console.WriteLine("[+] Deck reset successfully!");
    }
}
