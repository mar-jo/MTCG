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
    private static void ConnectDB()
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

    private static void CloseDB()
    {
        try
        {
            _conn.Close();
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    public static void InsertData()
    {
        ConnectDB();

        using (var command = new NpgsqlCommand("INSERT INTO inventory (name, quantity) VALUES (@n1, @q1), (@n2, @q2), (@n3, @q3)", _conn))
        {
            //TODO: Update Parameters
            command.Parameters.AddWithValue("n1", "banana");
            command.Parameters.AddWithValue("q1", 150);
            command.Parameters.AddWithValue("n2", "orange");
            command.Parameters.AddWithValue("q2", 154);
            command.Parameters.AddWithValue("n3", "apple");
            command.Parameters.AddWithValue("q3", 100);

            int nRows = command.ExecuteNonQuery();
            Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
        }

        CloseDB();
    }
}
