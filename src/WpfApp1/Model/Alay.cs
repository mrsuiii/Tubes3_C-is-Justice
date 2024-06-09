using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WpfApp1.Model
{
    public class Alay
    {
        // Mengubah alay ke normal
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

            return converted;
        }

        // Mengubah nama normal ke alay
        public string NormalToAlay(string namaNormal)
        {
            // Mengubah huruf menjadi angka sesuai dengan aturan alay
            Dictionary<char, char> letterToDigitMap = new Dictionary<char, char>
            {
                { 'o', '0' },
                { 'i', '1' },
                { 'z', '2' },
                { 'e', '3' },
                { 'a', '4' },
                { 's', '5' },
                { 'g', '6' },
                { 't', '7' },
                { 'b', '8' }
            };

            StringBuilder alayName = new StringBuilder();

            Random rand = new Random();
            string[] words = namaNormal.ToLower().Split(' ');

            foreach (string word in words)
            {
                string shortenedWord = ShortenWord(word);
                foreach (char c in shortenedWord)
                {
                    if (letterToDigitMap.ContainsKey(c))
                    {
                        alayName.Append(letterToDigitMap[c]);
                    }
                    else
                    {
                        // Sesekali mengubah huruf kecil menjadi huruf besar secara acak
                        if (char.IsLetter(c) && rand.Next(2) == 0)
                        {
                            alayName.Append(char.ToUpper(c));
                        }
                        else
                        {
                            alayName.Append(c);
                        }
                    }
                }
                alayName.Append(' ');
            }

            return alayName.ToString().Trim();
        }

        private string ShortenWord(string word)
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
    }
}
