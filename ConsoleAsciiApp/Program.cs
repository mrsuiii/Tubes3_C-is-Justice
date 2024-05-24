using System;
using System.Drawing;
using System.IO;
using System.Text;  // Tambahkan ini untuk StringBuilder
using MySql.Data.MySqlClient;
using ConsoleAsciiApp.Utilities;

namespace ConsoleAsciiApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "server=localhost;user=root;password=cu1747;database=fingerprint_db";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve path from database
                string query = "SELECT berkas_citra FROM sidik_jari";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string imagePath = reader.GetString("berkas_citra");

                            // Load image and convert to ASCII
                            Bitmap bitmap = LoadBitmap(imagePath);
                            string asciiArt = ConvertImageToAscii(bitmap, 100); // Width set to 100 for example

                            // Construct the Foto object
                            Foto foto = new Foto(imagePath, asciiArt);

                            // Print validation
                            PrintFotoValidation(foto);
                        }
                    }
                }
            }
        }

        static Bitmap LoadBitmap(string filePath)
        {
            return new Bitmap(filePath);
        }

        static string ConvertImageToAscii(Bitmap image, int width)
        {
            // Resize image to the specified width while maintaining aspect ratio
            int height = (int)((double)image.Height / image.Width * width);
            using (Bitmap resizedImage = new Bitmap(image, new Size(width, height)))
            {
                // Convert to grayscale
                using (Bitmap grayscaleImage = ConvertToGrayscale(resizedImage))
                {
                    // Convert to ASCII
                    StringBuilder asciiArt = new StringBuilder();
                    char[] asciiChars = { '@', '#', 'S', '%', '?', '*', '+', ';', ':', ',', '.' };
                    for (int y = 0; y < grayscaleImage.Height; y++)
                    {
                        for (int x = 0; x < grayscaleImage.Width; x++)
                        {
                            Color pixelColor = grayscaleImage.GetPixel(x, y);
                            int grayValue = pixelColor.R; // Since it's grayscale, R, G, and B values are the same
                            int asciiIndex = grayValue * (asciiChars.Length - 1) / 255;
                            asciiArt.Append(asciiChars[asciiIndex]);
                        }
                        asciiArt.AppendLine();
                    }

                    return asciiArt.ToString();
                }
            }
        }

        static Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayscale = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    grayscale.SetPixel(x, y, grayColor);
                }
            }

            return grayscale;
        }

        static void PrintFotoValidation(Foto foto)
        {
            Console.WriteLine("Validating Foto Object...");
            Console.WriteLine($"Path: {foto.Path}");
            Console.WriteLine("ASCII Representation:");
            Console.WriteLine(foto.AsciiRepresentation);
            Console.WriteLine("Validation Complete.");
        }
    }
}
