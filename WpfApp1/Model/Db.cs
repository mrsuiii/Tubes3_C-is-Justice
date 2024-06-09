using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Text;
using WpfApp1.Utilities;
using System.IO;
using System.Diagnostics;

namespace WpfApp1.Model
{
    public class Db
    {
        private string connectionString = "server=localhost;user=root;password=cu1747;database=fingerprint_db";
        private Foto[] fotos; // Array to store Foto objects
        private Biodata[] biodatas;
        private Alay alayconvert;

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
                            string imagePath = Path.Combine(basePath, "..\\..\\..\\", relativeImagePath);

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

        public void InsertImagePathsAndNames()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\fingerprint");
            if (!Directory.Exists(folderPath))
            {
                Debug.WriteLine("Folder not found.");
                return;
            }

            string[] files = Directory.GetFiles(folderPath, "*.BMP");
            Array.Sort(files); // Sort files alphabetically
            Debug.WriteLine(files[0]);

            // Generate 600 names
            List<string> names = GenerateNames(600);

            List<(string Path, string Name)> filePathNamePairs = new List<(string Path, string Name)>();

            // Assign names based on index
            for (int i = 0; i < files.Length; i++)
            {
                int nameIndex = i / 10; // Assign a new name every 10 files
                if (nameIndex < names.Count)
                {
                    string fileName = Path.GetFileName(files[i]);
                    string relativePath = Path.Combine("fingerprint", fileName).Replace("\\", "/");
                    string name = names[nameIndex];
                    //string asciiArt = "Asu";
                    filePathNamePairs.Add((relativePath, name));
                }
            }

            for (int i = 0; i < 600; i++)
            {
                InsertBiodata(names[i]);
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                foreach (var pair in filePathNamePairs)
                {
                    string query = "INSERT INTO sidik_jari (berkas_citra, nama) VALUES (@berkas_citra, @nama)";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@berkas_citra", pair.Path);
                        command.Parameters.AddWithValue("@nama", pair.Name);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void InsertBiodata(string nama)
        {
            Random rnd = new Random();

            Alay alayconvert = new Alay();
            nama = alayconvert.NormalToAlay(nama);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Prepare SQL statement for inserting biodata
                string insertQuery = "INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) " +
                                     "VALUES (@NIK, @nama, @tempat_lahir, @tanggal_lahir, @jenis_kelamin, @golongan_darah, @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)";

                // Iterate through the biodata list
                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    // Set random values for each attribute
                    command.Parameters.AddWithValue("@NIK", GenerateRandomNIK());
                    command.Parameters.AddWithValue("@nama", nama);
                    command.Parameters.AddWithValue("@tempat_lahir", GenerateRandomPlaceOfBirth());
                    command.Parameters.AddWithValue("@tanggal_lahir", GenerateRandomDateOfBirth().ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@jenis_kelamin", rnd.Next(2) == 0 ? "Laki-laki" : "Perempuan");
                    command.Parameters.AddWithValue("@golongan_darah", GenerateRandomBloodType());
                    command.Parameters.AddWithValue("@alamat", GenerateRandomAddress());
                    command.Parameters.AddWithValue("@agama", GenerateRandomReligion());
                    command.Parameters.AddWithValue("@status_perkawinan", GenerateRandomMaritalStatus());
                    command.Parameters.AddWithValue("@pekerjaan", GenerateRandomOccupation());
                    command.Parameters.AddWithValue("@kewarganegaraan", "WNI");

                    // Execute the SQL command
                    command.ExecuteNonQuery();
                }
            }

            Debug.WriteLine("Biodata inserted successfully.");
        }

        // Method to generate random 16-digit NIK number
        private string GenerateRandomNIK()
        {
            Random rnd = new Random();
            // Generate random 16-digit NIK number
            string nik = "";
            for (int i = 0; i < 16; i++)
            {
                nik += rnd.Next(10).ToString();
            }
            return nik;
        }

        // Method to generate random name
        private List<string> GenerateNames(int count)
        {
            string[] firstNames =
            {
                "Muhammad", "Aulia", "Ahmad", "Putri", "Andre", "Siti", "Agus", "Lina", "Budi", "Rini",
                "Bayu", "Dewi", "Hadi", "Ratna", "Farhan", "Fitri", "Rizki", "Nur", "Yoga", "Sari",
                "Eko", "Astuti", "Hendra", "Siska", "Irfan", "Dian", "Adi", "Sinta", "Galih", "Maya",
                "Surya", "Anita", "Joko", "Eka", "Aldi", "Citra", "Herry", "Rosa", "Hendro", "Dina",
                "Dwi", "Rahayu", "Ivan", "Nita", "Aditya", "Rina", "Faisal", "Rini", "Denny", "Novi"
            };

            string[] lastNames =
            {
                "Wibowo", "Sari", "Susanto", "Hadi", "Setiawan", "Kusuma", "Mulyadi", "Wijaya", "Budiman", "Purnama",
                "Nugroho", "Santoso", "Pangestu", "Yulianto", "Pratama", "Yuniarti", "Hermawan", "Suryanto", "Widodo", "Puspitasari",
                "Kurniawan", "Ningsih", "Saputra", "Rahayu", "Purnomo", "Sulistyo", "Putra", "Astuti", "Gunawan", "Agustina",
                "Hartanto", "Utami", "Saputri", "Wijaya", "Indriani", "Hidayat", "Anggraini", "Prasetyo", "Sari", "Wijaya",
                "Siregar", "Setiawan", "Wati", "Supriadi", "Nurhayati", "Saputra", "Handayani", "Saputra", "Lestari", "Santoso"
            };

            Random rand = new Random();
            List<string> names = new List<string>();

            for (int i = 0; i < count; i++)
            {
                string firstName = firstNames[rand.Next(firstNames.Length)];
                string lastName = lastNames[rand.Next(lastNames.Length)];
                names.Add($"{firstName} {lastName}");
            }

            return names;
        }

        // Method to generate random place of birth
        private string GenerateRandomPlaceOfBirth()
        {
            Random rnd = new Random();
            string[] places = {
                "Jakarta", "Bandung", "Surabaya", "Semarang", "Medan",
                "Denpasar", "Makassar", "Palembang", "Yogyakarta", "Malang",
                "Bogor", "Tangerang", "Depok", "Pekanbaru", "Padang",
                "Bandar Lampung", "Surakarta", "Banjarmasin", "Serang", "Balikpapan",
                "Pontianak", "Manado", "Jambi", "Mataram", "Banda Aceh",
                "Pekalongan", "Bengkulu", "Ambon", "Samarinda", "Palu"
            };
            return places[rnd.Next(places.Length)];
        }

        // Method to generate random date of birth between 1950 and 2003
        private DateTime GenerateRandomDateOfBirth()
        {
            Random rnd = new Random();
            DateTime startDate = new DateTime(1950, 1, 1);
            DateTime endDate = new DateTime(2003, 12, 31);
            int range = (endDate - startDate).Days;
            return startDate.AddDays(rnd.Next(range));
        }

        // Method to generate random blood type
        private string GenerateRandomBloodType()
        {
            Random rnd = new Random();
            string[] bloodTypes = { "A", "B", "AB", "O" };
            return bloodTypes[rnd.Next(bloodTypes.Length)];
        }

        // Method to generate random address
        private string GenerateRandomAddress()
        {
            Random rnd = new Random();
            string[] streets = {
                "Jl. Jendral Sudirman", "Jl. MH Thamrin", "Jl. Diponegoro",
                "Jl. Pahlawan", "Jl. Gajah Mada", "Jl. Imam Bonjol",
                "Jl. Veteran", "Jl. A. Yani", "Jl. Asia Afrika", "Jl. Raden Saleh",
                "Jl. Raya", "Jl. KH. Hasyim Asy'ari", "Jl. Pemuda", "Jl. Gatot Subroto",
                "Jl. Raya Bogor", "Jl. Cikutra", "Jl. Sukajadi", "Jl. Cimanuk",
                "Jl. Ki Mangunsarkoro", "Jl. Adisucipto", "Jl. Sisingamangaraja",
                "Jl. Soekarno-Hatta", "Jl. Pahlawan Revolusi", "Jl. Merdeka", "Jl. Sulawesi",
                "Jl. Mataram", "Jl. Piere Tendean", "Jl. Thamrin", "Jl. D.I. Panjaitan",
                "Jl. Dr. Wahidin", "Jl. Panglima Sudirman"
            };
            string[] cities = {
                "Jakarta", "Bandung", "Surabaya", "Semarang", "Medan",
                "Denpasar", "Makassar", "Palembang", "Yogyakarta", "Malang",
                "Bogor", "Tangerang", "Depok", "Pekanbaru", "Padang",
                "Bandar Lampung", "Surakarta", "Banjarmasin", "Serang", "Balikpapan",
                "Pontianak", "Manado", "Jambi", "Mataram", "Banda Aceh",
                "Pekalongan", "Bengkulu", "Ambon", "Samarinda", "Palu"
            };
            return $"{streets[rnd.Next(streets.Length)]}, {cities[rnd.Next(cities.Length)]}";
        }

        // Method to generate random religion
        private string GenerateRandomReligion()
        {
            Random rnd = new Random();
            string[] religions = { "Islam", "Kristen", "Katholik", "Hindu", "Buddha", "Konghucu" };
            return religions[rnd.Next(religions.Length)];
        }

        // Method to generate random marital status
        private string GenerateRandomMaritalStatus()
        {
            Random rnd = new Random();
            string[] maritalStatuses = { "Belum Menikah", "Menikah", "Cerai" };
            return maritalStatuses[rnd.Next(maritalStatuses.Length)];
        }

        // Method to generate random occupation
        private string GenerateRandomOccupation()
        {
            Random rnd = new Random();
            string[] occupations = { "Mahasiswa", "Wiraswasta", "Pegawai Swasta", "PNS", "Guru", "Dokter", "Pengusaha", "Pilot", "Peneliti", "Seniman" };
            return occupations[rnd.Next(occupations.Length)];
        }
        public int FotosLength()
        {
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