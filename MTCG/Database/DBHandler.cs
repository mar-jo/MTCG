using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        // TODO: Return 201 if successful and 409 if not.
        ConnectDB();

        var cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", _conn);
        cmd.Parameters.AddWithValue("username", user["Username"]);
        cmd.Parameters.AddWithValue("password", user["Password"]);
        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("[+] User created successfully!");
            return new HttpResponseMessage(HttpStatusCode.Created);
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
