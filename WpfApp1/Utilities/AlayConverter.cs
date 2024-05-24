using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace WpfApp1.Utilities
{
    public class AlayConverter
    {
        static string AlayTransform(string namaAlay)
        {
            // mengubah semua angka alay menjadi huruf
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
            string angkaAlay = "[0123456789]";
            string converted = Regex.Replace(namaAlay, angkaAlay, match => { return digitToLetterMap[match.Value[0]].ToString(); });

            // mengubah semua huruf besar menjadi huruf kecil
            string hurufBesarAlay = "[A-Z]";
            converted = Regex.Replace(converted, hurufBesarAlay, match => match.Value.ToLower());

            // idk apakah penyingkatan dan spasi perlu dihandle

            return converted;
        }

    }
}