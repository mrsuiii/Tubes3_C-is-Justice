using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WpfApp1.Utilities;
using System.Drawing;

namespace WpfApp1.Model
{
    public class BM
    {
        private readonly ImageToAsciiConverter _converter;
        public BM()
        {
            _converter = new ImageToAsciiConverter();
        }

        // menerima gambar target, dan array gambar dari database

        public int BoyerMooreSearch(string text, string pattern)
        {
            int[] charTable = MakeCharTable(pattern);
            int[] offsetTable = MakeOffsetTable(pattern);
            for (int i = pattern.Length - 1, j; i < text.Length;)
            {
                for (j = pattern.Length - 1; pattern[j] == text[i]; --i, --j)
                {
                    if (j == 0)
                        return i;
                }
                i += Math.Max(offsetTable[pattern.Length - 1 - j], charTable[text[i]]);
            }
            return -1;
        }

        private int[] MakeCharTable(string pattern)
        {
            const int ALPHABET_SIZE = 256;
            int[] table = new int[ALPHABET_SIZE];
            for (int i = 0; i < table.Length; ++i)
            {
                table[i] = pattern.Length;
            }
            for (int i = 0; i < pattern.Length - 1; ++i)
            {
                table[pattern[i]] = pattern.Length - 1 - i;
            }
            return table;
        }

        private int[] MakeOffsetTable(string pattern)
        {
            int[] table = new int[pattern.Length];
            int lastPrefixPosition = pattern.Length;
            for (int i = pattern.Length - 1; i >= 0; --i)
            {
                if (IsPrefix(pattern, i + 1))
                {
                    lastPrefixPosition = i + 1;
                }
                table[pattern.Length - 1 - i] = lastPrefixPosition - i + pattern.Length - 1;
            }
            for (int i = 0; i < pattern.Length - 1; ++i)
            {
                int slen = SuffixLength(pattern, i);
                table[slen] = pattern.Length - 1 - i + slen;
            }
            return table;
        }

        private bool IsPrefix(string pattern, int p)
        {
            for (int i = p, j = 0; i < pattern.Length; ++i, ++j)
            {
                if (pattern[i] != pattern[j])
                {
                    return false;
                }
            }
            return true;
        }

        private int SuffixLength(string pattern, int p)
        {
            int len = 0;
            for (int i = p, j = pattern.Length - 1; i >= 0 && pattern[i] == pattern[j]; --i, --j)
            {
                len += 1;
            }
            return len;
        }
        
    }

}