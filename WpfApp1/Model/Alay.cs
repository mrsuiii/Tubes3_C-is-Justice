using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class Alay
    {
        public string AlayTransform(string namaAlay)
        {
            // Mengubah semua angka alay menjadi huruf
            Dictionary<char, char> digitToLetterMap = new Dictionary<char, char>
        {
            { '0', 'o' },
            { '1', 'i' },
            { '2', 'z' },
            { '3', 'e' },
            { '4', 'a' },
            { '5', 's' },
            { '6', 'g' },
            { '7', 't' },
            { '8', 'b' },
            { '9', 'g' }
        };

            // Ganti angka dengan huruf yang bersesuaian
            string angkaAlay = "[0123456789]";
            string converted = Regex.Replace(namaAlay, angkaAlay, match => digitToLetterMap[match.Value[0]].ToString());

            // Mengubah semua huruf besar menjadi huruf kecil
            string hurufBesarAlay = "[A-Z]";
            converted = Regex.Replace(converted, hurufBesarAlay, match => match.Value.ToLower());

            // Penyingkatan kata (menghapus huruf vokal kecuali huruf pertama dari setiap kata)
            /*
            string[] words = converted.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = ShortenWord(words[i]);
            }
            converted = string.Join(" ", words);
            */
            return converted;
            
        }

        /*
        private static string ShortenWord(string word)
        {
            if (word.Length <= 1) return word;
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            string shortened = word[0].ToString(); // Simpan huruf pertama

            for (int i = 1; i < word.Length; i++)
            {
                if (Array.IndexOf(vowels, word[i]) == -1)
                {
                    shortened += word[i]; // Tambahkan hanya konsonan
                }
            }

            return shortened;
        }
        */
    }
}
