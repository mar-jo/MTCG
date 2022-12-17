using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MTCG.Database;

public class DBHandler
{
    private const string ConnString = "Host=localhost;Username=postgres;Password=admin;Database=MTCG";
    private static NpgsqlConnection? _conn;
    public static void Connect()
    {
        try
        {
            _conn = new NpgsqlConnection(ConnString);
            Console.Out.WriteLine("Opening connection...");
            _conn.Open();
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    public static void HandleRequest(char input)
    {

    }
}
