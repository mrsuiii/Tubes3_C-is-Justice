using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Utilities
{
    public class Biodata
    {
        public string NIK { get; set; } // varchar(16), primary key, not null
        public string Nama { get; set; } // varchar(100), nullable
        public string TempatLahir { get; set; } // varchar(50), nullable
        public string TanggalLahir { get; set; } // date, nullable, disimpan sebagai string
        public string JenisKelamin { get; set; } // enum('Laki-Laki','Perempuan'), nullable
        public string GolonganDarah { get; set; } // varchar(5), nullable
        public string Alamat { get; set; } // varchar(255), nullable
        public string Agama { get; set; } // varchar(50), nullable
        public string StatusPerkawinan { get; set; } // enum('Belum Menikah','Menikah','Cerai'), nullable
        public string Pekerjaan { get; set; } // varchar(100), nullable
        public string Kewarganegaraan { get; set; } // varchar(50), nullable

        // Constructor
        public Biodata(string nik, string nama = null, string tempatLahir = null, string tanggalLahir = null,
                       string jenisKelamin = null, string golonganDarah = null, string alamat = null,
                       string agama = null, string statusPerkawinan = null, string pekerjaan = null,
                       string kewarganegaraan = null)
        {
            NIK = nik;
            Nama = nama;
            TempatLahir = tempatLahir;
            TanggalLahir = tanggalLahir;
            JenisKelamin = jenisKelamin;
            GolonganDarah = golonganDarah;
            Alamat = alamat;
            Agama = agama;
            StatusPerkawinan = statusPerkawinan;
            Pekerjaan = pekerjaan;
            Kewarganegaraan = kewarganegaraan;
        }

        // Override ToString method to display biodata information
        public override string ToString()
        {
            return $"NIK: {NIK}, Nama: {Nama}, Tempat Lahir: {TempatLahir}, Tanggal Lahir: {TanggalLahir ?? "Tanggal lahir tidak tersedia"}, " +
                   $"Jenis Kelamin: {JenisKelamin}, Golongan Darah: {GolonganDarah}, Alamat: {Alamat}, Agama: {Agama}, " +
                   $"Status Perkawinan: {StatusPerkawinan}, Pekerjaan: {Pekerjaan}, Kewarganegaraan: {Kewarganegaraan}";
        }
    }

}
