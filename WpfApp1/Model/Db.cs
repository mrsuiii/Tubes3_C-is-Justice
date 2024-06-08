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
        private Biodata[] biodatas;

        public void ProcessImages()
        {
            List<Foto> fotoList = new List<Foto>();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve path from database
                string query = "SELECT berkas_citra, nama FROM sidik_jari";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string relativeImagePath = reader.GetString("berkas_citra");
                            string CorruptName = reader.GetString("nama");
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
                                Foto foto = new Foto(imagePath, asciiArt, CorruptName);

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

        public void ProcessBiodata()
        {
            List<Biodata> biodataList = new List<Biodata>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve biodata from database
                string query = "SELECT NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan FROM biodata";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                // Read values from database
                                string nik = reader.GetString("NIK");
                                string nama = reader.IsDBNull(reader.GetOrdinal("nama")) ? null : reader.GetString("nama");
                                string tempatLahir = reader.IsDBNull(reader.GetOrdinal("tempat_lahir")) ? null : reader.GetString("tempat_lahir");
                                string tanggalLahir = reader.IsDBNull(reader.GetOrdinal("tanggal_lahir")) ? null : reader.GetDateTime("tanggal_lahir").ToString("yyyy-MM-dd");
                                string jenisKelamin = reader.IsDBNull(reader.GetOrdinal("jenis_kelamin")) ? null : reader.GetString("jenis_kelamin");
                                string golonganDarah = reader.IsDBNull(reader.GetOrdinal("golongan_darah")) ? null : reader.GetString("golongan_darah");
                                string alamat = reader.IsDBNull(reader.GetOrdinal("alamat")) ? null : reader.GetString("alamat");
                                string agama = reader.IsDBNull(reader.GetOrdinal("agama")) ? null : reader.GetString("agama");
                                string statusPerkawinan = reader.IsDBNull(reader.GetOrdinal("status_perkawinan")) ? null : reader.GetString("status_perkawinan");
                                string pekerjaan = reader.IsDBNull(reader.GetOrdinal("pekerjaan")) ? null : reader.GetString("pekerjaan");
                                string kewarganegaraan = reader.IsDBNull(reader.GetOrdinal("kewarganegaraan")) ? null : reader.GetString("kewarganegaraan");

                                // Construct the Biodata object
                                Biodata biodata = new Biodata(nik, nama, tempatLahir, tanggalLahir, jenisKelamin, golonganDarah, alamat, agama, statusPerkawinan, pekerjaan, kewarganegaraan);

                                // Add Biodata object to the list
                                biodataList.Add(biodata);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing biodata entry: {ex.Message}");
                                continue; // Skip this entry and move to the next
                            }
                        }
                    }
                }
            }

            // Convert list to array
            biodatas = biodataList.ToArray();
        }

        public int FotosLength() { 
            return fotos.Length;
            
        }
        public Foto[] GetFotos()
        {
            return fotos;
        }

        public Biodata[] GetBiodatas()
        {
            return biodatas;
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
