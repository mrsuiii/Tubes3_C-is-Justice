using System;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "Server=localhost;User ID=root;Database=fingerprint_db;Password=174756";

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string query = "SELECT nama FROM biodata";
            using var cmd = new MySqlCommand(query, conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["nama"].ToString());
            }
        }

        // Fungsi untuk mencari nama di database dengan handling variasi alay
        public static string FindNameInDatabase(string name, MySqlConnection conn)
        {
            string normalizedName = AlayToAlphabetic(name);
            string query = "SELECT nama FROM biodata";
            using var cmd = new MySqlCommand(query, conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string storedName = reader.GetString(0);
                if (KMPSearch(storedName.ToLower(), normalizedName.ToLower()) != -1 ||
                    BoyerMooreSearch(storedName.ToLower(), normalizedName.ToLower()) != -1)
                {
                    return storedName;
                }
            }
            return null;
        }

        // Fungsi konversi nama alay ke bentuk alfabetik
        public static string AlayToAlphabetic(string name)
        {
            var alayPatterns = new (string, string)[]
            {
                ("1", "i"), ("4", "a"), ("6", "g"), ("0", "o"), ("3", "e"), ("5", "s"), ("7", "t"), ("8", "b"),
                ("b1", "B"), ("1n", "in"), ("m4", "Mar")
            };
            foreach (var pattern in alayPatterns)
            {
                name = Regex.Replace(name, "(?i)" + pattern.Item1, pattern.Item2);
            }
            return name;
        }

        // Algoritma KMP
        public static int KMPSearch(string text, string pattern)
        {
            int[] lps = ComputeLpsArray(pattern);
            int i = 0, j = 0;
            while (i < text.Length)
            {
                if (pattern[j] == text[i])
                {
                    i++;
                    j++;
                }
                if (j == pattern.Length)
                {
                    return i - j;
                }
                else if (i < text.Length && pattern[j] != text[i])
                {
                    if (j != 0)
                    {
                        j = lps[j - 1];
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return -1;
        }

        public static int[] ComputeLpsArray(string pattern)
        {
            int length = 0;
            int i = 1;
            int[] lps = new int[pattern.Length];
            lps[0] = 0;
            while (i < pattern.Length)
            {
                if (pattern[i] == pattern[length])
                {
                    length++;
                    lps[i] = length;
                    i++;
                }
                else
                {
                    if (length != 0)
                    {
                        length = lps[length - 1];
                    }
                    else
                    {
                        lps[i] = 0;
                        i++;
                    }
                }
            }
            return lps;
        }

        // Algoritma Boyer-Moore
        public static int BoyerMooreSearch(string text, string pattern)
        {
            int[] last = BuildLast(pattern);
            int n = text.Length;
            int m = pattern.Length;
            int i = m - 1;

            if (i > n - 1)
            {
                return -1;
            }

            int j = m - 1;
            do
            {
                if (pattern[j] == text[i])
                {
                    if (j == 0)
                    {
                        return i;
                    }
                    else
                    {
                        i--;
                        j--;
                    }
                }
                else
                {
                    int lo = last[text[i]];
                    i = i + m - Math.Min(j, 1 + lo);
                    j = m - 1;
                }
            } while (i <= n - 1);

            return -1;
        }

        private static int[] BuildLast(string pattern)
        {
            int[] last = new int[256];
            for (int i = 0; i < 256; i++)
            {
                last[i] = -1;
            }
            for (int i = 0; i < pattern.Length; i++)
            {
                last[pattern[i]] = i;
            }
            return last;
        }

        // Metode lainnya jika diperlukan
    }
}
