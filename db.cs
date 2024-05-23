using System;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=localhost;Database=fingerprint_db;Uid=root;Pwd=password;";
        
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                
                string query = "SELECT nama FROM biodata;";
                MySqlCommand command = new MySqlCommand(query, connection);
                
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nama = reader.GetString("nama");
                        Console.WriteLine("Nama: " + nama);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
