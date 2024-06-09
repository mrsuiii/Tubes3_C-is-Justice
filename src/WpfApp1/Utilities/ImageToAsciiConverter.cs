using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WpfApp1.Utilities
{
    public class ImageToAsciiConverter
    {
        public string ConvertImageToAscii(Bitmap image)
        {
            
            string binaryImage = ConvertImageToBinary(image);

            StringBuilder asciiArt = new StringBuilder();

            asciiArt.Append(BinaryToAscii(binaryImage));
            Debug.WriteLine(asciiArt.ToString());
            return asciiArt.ToString();
        }
        public string[] ConvertImageToAsciiArray(Bitmap image)
        {
            // Mengonversi gambar ke string biner
            string binaryString = ConvertImageToBinary(image);

            // Mengambil substring 64-bit terbaik dari string biner
           
            string asciiString = BinaryToAscii(binaryString);
            string[] best64BitSubstrings = GetBestSubstrings(asciiString, 16, 25);
            // Mengonversi setiap substring biner menjadi ASCII
            string[] asciiArtArray = new string[best64BitSubstrings.Length];
            for (int i = 0; i < best64BitSubstrings.Length; i++)
            {
                asciiArtArray[i] = best64BitSubstrings[i];
            }
          
            return asciiArtArray;
        }
        public Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                using (var bitmap = new Bitmap(outStream))
                {
                    return new Bitmap(bitmap);
                }
            }
        }

        public string ConvertImageToBinary(Bitmap image)
        {
            // Mengonversi ke grayscale
            Bitmap grayscaleImage = ConvertToGrayscale(image);

            // Mengonversi ke string biner menggunakan ambang batas
            StringBuilder binaryString = new StringBuilder();
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(x, y);
                    int grayValue = pixelColor.R; // Karena grayscale, nilai R, G, dan B sama
                    binaryString.Append(grayValue > 127 ? '1' : '0');
                }
            }
            Debug.WriteLine(binaryString.ToString());
            Debug.WriteLine(binaryString.ToString().Length);
            return binaryString.ToString();
        }

        public string[] GetBest64BitSubstrings(string binaryString, int count =10)
        {
            if (binaryString.Length < 64)
            {
                throw new ArgumentException("String biner terlalu pendek untuk mengambil substring 64-bit.");
            }

            var substrings = new List<(string, double)>();

            for (int i = 0; i <= binaryString.Length - 512; i++)
            {
                string substring = binaryString.Substring(i, 512);
                double uniformityScore = EvaluateUniformity(substring);
                substrings.Add((substring, uniformityScore));

            }
            // Mengurutkan substring berdasarkan skor uniformitas (yang paling dekat dengan 0.5)
            var bestSubstrings = substrings.OrderBy(s => Math.Abs(s.Item2 - 0.5)).Take(count).Select(s => s.Item1).ToArray();
            Debug.WriteLine(bestSubstrings);
            return bestSubstrings;
        }

        private double EvaluateUniformity(string substring)
        {
            // Menghitung rasio 1s terhadap panjang total
            int onesCount = substring.Count(c => c == '1');
            return (double)onesCount / substring.Length;
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

        public string BinaryToAscii(string binaryString)
        {
            // Ensure the binary string only contains '0' and '1'
            if (binaryString.Any(c => c != '0' && c != '1'))
            {
                throw new ArgumentException("Binary string can only contain '0' and '1'.");
            }

            char[] asciiChars = { '@', '#', 'S', '%', '?', '*', '+', ';', ':', ',', '.' };

            StringBuilder asciiString = new StringBuilder();

            // Ensure the length of the binary string is a multiple of 8
            int paddingLength = 8 - (binaryString.Length % 8);
            if (paddingLength != 8)
            {
                binaryString = binaryString.PadRight(binaryString.Length + paddingLength, '0');
            }

            for (int i = 0; i < binaryString.Length; i += 8)
            {
                string byteString = binaryString.Substring(i, 8);
                int asciiValue = Convert.ToInt32(byteString, 2);

                // Map the ASCII value to the predefined ASCII characters used for ASCII art
                char asciiChar;
                if (asciiValue < asciiChars.Length)
                {
                    asciiChar = asciiChars[asciiValue % asciiChars.Length];
                }
                else
                {
                    asciiChar = '.'; // Default character if out of range
                }

                asciiString.Append(asciiChar);
            }

            return asciiString.ToString();
        }

        private double EvaluateFrequency(string substring)
        {
            int changes = 0;
            for (int i = 0; i < substring.Length - 1; i++)
            {

                if (substring[i] != substring[i + 1])
                {
                    changes++;
                }
            }
            return (double)changes / (substring.Length - 1);
        }

        private double EvaluateEntropy(string substring)
        {
            int zeroCount = substring.Count(c => c == '0');
            int oneCount = substring.Count(c => c == '1');

            double zeroProbability = (double)zeroCount / substring.Length;
            double oneProbability = (double)oneCount / substring.Length;

            double entropy = 0.0;
            if (zeroProbability > 0)
            {
                entropy -= zeroProbability * Math.Log(zeroProbability, 2);
            }
            if (oneProbability > 0)
            {
                entropy -= oneProbability * Math.Log(oneProbability, 2);
            }

            return entropy;
        }
        private double EvaluateRunLengthEncoding(string substring)
        {
            List<int> runs = new List<int>();
            char currentChar = substring[0];
            int runLength = 1;

            for (int i = 1; i < substring.Length; i++)
            {
                if (substring[i] == currentChar)
                {
                    runLength++;
                }
                else
                {
                    runs.Add(runLength);
                    currentChar = substring[i];
                    runLength = 1;
                }
            }
            runs.Add(runLength);

            // Menghitung rata-rata panjang run
            double averageRunLength = runs.Average();
            return averageRunLength;
        }

        public string[] GetBestSubstrings(string binaryString, int substringLength, int count = 10)
        {
            if (binaryString.Length < substringLength)
            {
                throw new ArgumentException($"Binary string is too short to extract {substringLength}-bit substrings.");
            }

            var substrings = new List<(string, double)>();

            // Menggunakan sliding window untuk memeriksa semua kemungkinan substring
            Parallel.For(0, binaryString.Length - substringLength + 1, i =>
            {
                string substring = binaryString.Substring(i, substringLength);
                double uniformityScore = EvaluateUniformity(substring);
                double frequencyScore = EvaluateFrequency(substring);
                double entropyScore = EvaluateEntropy(substring);
                double runLengthScore = EvaluateRunLengthEncoding(substring);
                double combinedScore = CombineScores(uniformityScore, frequencyScore, entropyScore, runLengthScore);

                lock (substrings)
                {
                    substrings.Add((substring, combinedScore));
                }
            });

            var bestSubstrings = substrings.OrderBy(s => s.Item2).Take(count).Select(s => s.Item1).ToArray();
            return bestSubstrings;
        }
        public string[] GetBestSubstringsByEntropy(string binaryString, int substringLength, int count = 3)
        {
            if (binaryString.Length < substringLength)
            {
                throw new ArgumentException($"Binary string is too short to extract {substringLength}-bit substrings.");
            }

            var substrings = new List<(string, double)>();

            // Menggunakan sliding window untuk memeriksa semua kemungkinan substring
            for (int i = 0; i <= binaryString.Length - substringLength; i++)
            {
                string substring = binaryString.Substring(i, substringLength);
                double entropyScore = EvaluateEntropy(substring);

                substrings.Add((substring, entropyScore));
            }

            // Mengurutkan substring berdasarkan skor entropy (yang tertinggi)
            var bestSubstrings = substrings.OrderByDescending(s => s.Item2).Take(count).Select(s => s.Item1).ToArray();
            return bestSubstrings;
        }

        

        private double CombineScores(double uniformityScore, double frequencyScore, double entropyScore, double runLengthScore)
        {
            // Kombinasikan skor dengan bobot yang berbeda jika diperlukan
            return uniformityScore + frequencyScore + entropyScore + runLengthScore;
        }
        public string[] GetBestSubstringsByFrequency(string binaryString, int substringLength, int count = 3)
        {
            if (binaryString.Length < substringLength)
            {
                throw new ArgumentException($"Binary string is too short to extract {substringLength}-bit substrings.");
            }

            var substrings = new List<(string, double)>();

            // Menggunakan sliding window untuk memeriksa semua kemungkinan substring
            for (int i = 0; i <= binaryString.Length - substringLength; i++)
            {
                string substring = binaryString.Substring(i, substringLength);
                double frequencyScore = EvaluateFrequency(substring);

                substrings.Add((substring, frequencyScore));
            }

            // Mengurutkan substring berdasarkan skor frekuensi (yang tertinggi)
            var bestSubstrings = substrings.OrderByDescending(s => s.Item2).Take(count).Select(s => s.Item1).ToArray();
            return bestSubstrings;
        }
    }
}