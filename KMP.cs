using System;

namespace KMPExample
{
    class Program
    {
        public static int KMPSearch(string text, string pattern)
        {
            int[] lps = BorderKMP(pattern);
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

        private static int[] BorderKMP(string pattern)
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

        static void Main(string[] args)
        {
            // Contoh pertama
            string text = "ABC ABCDAB ABCDABCDABDE";
            string pattern = "ABCDABD";
            int result = KMPSearch(text, pattern);
            Console.WriteLine(result != -1
                ? $"Pattern found at index {result}"
                : "Pattern not found");

            // Tambahan untuk perbandingan ASCII
            text = "abc abcdab abcdabcdabde";
            pattern = "abcdabd";
            result = KMPSearch(text, pattern);
            Console.WriteLine(result != -1
                ? $"Pattern found at index {result}"
                : "Pattern not found");

            Console.ReadLine();
        }
    }
}
