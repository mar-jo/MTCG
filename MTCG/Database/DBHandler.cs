﻿using System;
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

    //public static int AcquirePackage(Dictionary<string, string> user)
    //{
    //    if (!MessageHandler.IsAuthorized(user))
    //    {
    //        Console.WriteLine("[-] Invalid access-token...");
    //        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
    //    }
    //    else
    //    {
    //        // Access users table and check if coins are below 5, if so return 400
    //
    //        ConnectDB();
    //        var cmd = new NpgsqlCommand("SELECT coins FROM users WHERE username = @username", _conn);
    //        cmd.Parameters.AddWithValue("username", user["Username"]);
    //        cmd.Prepare();
    //        var reader = cmd.ExecuteReader();
    //        int coins = 0;
    //
    //        while (reader.Read())
    //        {
    //            coins = (int)reader["coins"];
    //        }
    //
    //        if (coins < 5)
    //        {
    //            Console.WriteLine("[-] Not enough coins left in wallet...");
    //            CloseDB();
    //            return new HttpResponseMessage(HttpStatusCode.Forbidden);
    //        }
    //
    //        cmd = new NpgsqlCommand("UPDATE users SET coins = coins - 5 WHERE username = @username", _conn);
    //        cmd.Parameters.AddWithValue("username", user["Username"]);
    //        cmd.Prepare();
    //        cmd.ExecuteNonQuery();
    //
    //        
    //    }
    //}

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
