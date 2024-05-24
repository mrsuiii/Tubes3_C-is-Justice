using System;
using System.Drawing;
using System.IO;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace ConsoleAsciiApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "server=localhost;user=root;password=cu1747;database=fingerprint_db";
            List<string> names = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Step 1: Read names from biodata
                string query = "SELECT nama FROM biodata";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            names.Add(reader.GetString("nama"));
                        }
                    }
                }

                // Step 2: Insert into sidik_jari with modified names
                foreach (string name in names)
                {
                    string modifiedName = ModifyName(name);
                    string imagePath = $"fingerprint/{name.Replace(" ", "_")}.BMP";
                    InsertIntoSidikJari(connection, imagePath, modifiedName);
                }
            }
        }

        static string ModifyName(string originalName)
        {
            // Example logic to convert to form "b1ntN6 Dw mrthn"
            string modified = originalName
                .Replace("i", "1")
                .Replace("a", "4")
                .Replace("e", "3")
                .Replace("o", "0")
                .Replace("s", "5")
                .Replace("g", "9");

            // Simple logic to randomize case and remove some characters
            Random rand = new Random();
            char[] array = modified.ToLower().ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (rand.Next(2) == 0)
                {
                    array[i] = Char.ToUpper(array[i]);
                }

                // Randomly skip some characters to simulate abbreviation
                if (rand.Next(5) == 0)
                {
                    array[i] = '\0';
                }
            }

            return new string(array).Replace("\0", "");
        }

        static void InsertIntoSidikJari(MySqlConnection connection, string imagePath, string name)
        {
            string query = "INSERT INTO sidik_jari (berkas_citra, nama) VALUES (@berkas_citra, @nama)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@berkas_citra", imagePath);
                command.Parameters.AddWithValue("@nama", name);
                command.ExecuteNonQuery();
            }
        }
    }
}
