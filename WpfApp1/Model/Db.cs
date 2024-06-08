using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Text;
using  WpfApp1.Utilities;
using System.IO;
using System.Diagnostics;

namespace WpfApp1.Model
{
    public class Db
    {
        private string connectionString = "server=localhost;user=root;password=emeryganteng;database=tubes3";
        private Foto[] fotos; // Array to store Foto objects

        public void ProcessImages()
        {
            List<Foto> fotoList = new List<Foto>();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

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
                            string relativeImagePath = reader.GetString("berkas_citra");
                            relativeImagePath = relativeImagePath.Replace("/", "\\");
                            string imagePath = Path.Combine(basePath,"..\\..\\..\\", relativeImagePath);

                            // Combine base path with the relative path to get the full path
                            // Validate the image path
                            if (!File.Exists(imagePath))
                            {
                                Debug.WriteLine($"Error: File does not exist at path {imagePath}");
                                continue; // Skip this entry and move to the next
                            }
                            try
                            {
                                // Load image and convert to ASCII
                                Bitmap bitmap = LoadBitmap(imagePath);
                                string asciiArt = ConvertImageToAscii(bitmap, 100); // Width set to 100 for example

                                // Construct the Foto object
                                Foto foto = new Foto(imagePath, asciiArt);

                                // Add Foto object to the list
                                fotoList.Add(foto);

                                // Print validation
                                Debug.WriteLine(fotoList[fotoList.Count - 1].getPath());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing image at path {imagePath}: {ex.Message}");
                                continue; // Skip this entry and move to the next
                            }
                        }
                    }
                }
            }
                        // Convert list to array
                        fotos = fotoList.ToArray();
        }
        public int FotosLength() { 
            return fotos.Length;
            
        }
        public Foto[] GetFotos()
        {
            return fotos;
        }

        private Bitmap LoadBitmap(string filePath)
        {
            return new Bitmap(filePath);
        }

        private string ConvertImageToAscii(Bitmap image, int width)
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

        private Bitmap ConvertToGrayscale(Bitmap original)
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

        private void PrintFotoValidation(Foto foto)
        {
            
        }
    }
}
