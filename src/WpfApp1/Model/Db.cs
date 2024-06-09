using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Text;
using WpfApp1.Utilities;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace WpfApp1.Model
{
    public class Db
    {
        private string connectionString = "server=localhost;user=root;password=cu1747;database=fingerprint_db";
        private Foto[] fotos; // Array to store Foto objects
        private Biodata[] biodatas;
        private Alay alayconvert;
        private ImageToAsciiConverter _converter;
        public void ProcessImages()
        {
            List<Foto> fotoList = new List<Foto>();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            _converter = new ImageToAsciiConverter();
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
                                string asciiArt = _converter.ConvertImageToAscii(bitmap); 

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

            byte[] key = new byte[] {
                0x00, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B,
                0x0C, 0x0D, 0x0E, 0x0F
            };

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
                                string nik = AES.aes128_decrypt(reader.GetString("NIK"), key);
                                string nama = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("nama")) ? null : reader.GetString("nama"), key);
                                string tempatLahir = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("tempat_lahir")) ? null : reader.GetString("tempat_lahir"), key);
                                string tanggalLahir = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("tanggal_lahir")) ? null : reader.GetDateTime("tanggal_lahir").ToString("yyyy-MM-dd"), key);
                                string jenisKelamin = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("jenis_kelamin")) ? null : reader.GetString("jenis_kelamin"), key);
                                string golonganDarah = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("golongan_darah")) ? null : reader.GetString("golongan_darah"), key);
                                string alamat = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("alamat")) ? null : reader.GetString("alamat"),key);
                                string agama = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("agama")) ? null : reader.GetString("agama"), key);
                                string statusPerkawinan = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("status_perkawinan")) ? null : reader.GetString("status_perkawinan"), key);
                                string pekerjaan = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("pekerjaan")) ? null : reader.GetString("pekerjaan"), key);
                                string kewarganegaraan = AES.aes128_decrypt(reader.IsDBNull(reader.GetOrdinal("kewarganegaraan")) ? null : reader.GetString("kewarganegaraan"), key);

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
            Debug.WriteLine("Starting the InsertImagePathsAndNames function.");

            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\fingerprint");
            if (!Directory.Exists(folderPath))
            {
                Debug.WriteLine("Folder not found.");
                return;
            }

            string[] files = Directory.GetFiles(folderPath, "*.BMP");
            // Sort files based on the numeric value in their name
            files = files.OrderBy(f => ExtractLeadingNumber(Path.GetFileName(f))).ThenBy(f => f).ToArray();
            Debug.WriteLine(files.Length);

            // Generate 600 names
            List<string> names = GenerateNames(600);

            // Assign names based on index
            for (int i = 0; i < 40; i++)
            {
                int nameIndex = i / 10;
                Debug.WriteLine(nameIndex);
                if (nameIndex < names.Count)
                {
                    string fileName = Path.GetFileName(files[i]);
                    string relativePath = Path.Combine("fingerprint", fileName).Replace("\\", "/");
                    string name = names[nameIndex];
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO sidik_jari (berkas_citra, nama) VALUES (@berkas_citra, @nama)";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@berkas_citra", relativePath);
                            command.Parameters.AddWithValue("@nama", name);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                InsertBiodata(names[i]);
            }
        }

        public void InsertBiodata(string nama)
        {
            Random rnd = new Random();

            alayconvert = new Alay();
            nama = alayconvert.NormalToAlay(nama);

            byte[] key = new byte[] {
                0x00, 0x01, 0x02, 0x03,
                0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0A, 0x0B,
                0x0C, 0x0D, 0x0E, 0x0F
            };

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
                    command.Parameters.AddWithValue("@NIK", AES.aes128_encrypt(GenerateRandomNIK(), key));
                    command.Parameters.AddWithValue("@nama", AES.aes128_encrypt(nama, key));
                    command.Parameters.AddWithValue("@tempat_lahir", AES.aes128_encrypt(GenerateRandomPlaceOfBirth(), key));
                    command.Parameters.AddWithValue("@tanggal_lahir", AES.aes128_encrypt(GenerateRandomDateOfBirth().ToString("yyyy-MM-dd"), key));
                    command.Parameters.AddWithValue("@jenis_kelamin", rnd.Next(2) == 0 ? "Laki-laki" : "Perempuan");
                    command.Parameters.AddWithValue("@golongan_darah", AES.aes128_encrypt(GenerateRandomBloodType(), key));
                    command.Parameters.AddWithValue("@alamat", AES.aes128_encrypt(GenerateRandomAddress(), key));
                    command.Parameters.AddWithValue("@agama", AES.aes128_encrypt(GenerateRandomReligion(), key));
                    command.Parameters.AddWithValue("@status_perkawinan",GenerateRandomMaritalStatus());
                    command.Parameters.AddWithValue("@pekerjaan", AES.aes128_encrypt(GenerateRandomOccupation(), key));
                    command.Parameters.AddWithValue("@kewarganegaraan", AES.aes128_encrypt("WNI", key));

                    // Execute the SQL command
                    command.ExecuteNonQuery();
                }
            }

            Debug.WriteLine("Biodata inserted successfully.");
        }

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

        private static int ExtractLeadingNumber(string fileName)
        {
            // Use regular expression to extract the leading number from the file name
            var match = Regex.Match(fileName, @"^\d+");
            return match.Success ? int.Parse(match.Value) : int.MaxValue;
        }
        // Method to generate random name
        private List<string> GenerateNames(int count)
        {
            string[] firstNames =
            {
                "Eka", "Aulia", "Ahmad", "Putri", "Andre", "Siti", "Agus", "Lina", "Budi", "Rini",
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