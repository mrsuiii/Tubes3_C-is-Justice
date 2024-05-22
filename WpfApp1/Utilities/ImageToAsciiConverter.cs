﻿using System;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace WpfApp1.Utilities
{
    public class ImageToAsciiConverter
    {
        private static readonly char[] AsciiChars = { '@', '#', 'S', '%', '?', '*', '+', ';', ':', ',', '.' };
        public  Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
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
        public string ConvertImageToAscii(Bitmap image, int width)
        {
            // Resize image to the specified width while maintaining aspect ratio
            int height = (int)((double)image.Height / image.Width * width);
            Bitmap resizedImage = new Bitmap(image, new Size(width, height));

            // Convert to grayscale
            Bitmap grayscaleImage = ConvertToGrayscale(resizedImage);

            // Convert to ASCII
            StringBuilder asciiArt = new StringBuilder();
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    Color pixelColor = grayscaleImage.GetPixel(x, y);
                    int grayValue = pixelColor.R; // Since it's grayscale, R, G, and B values are the same
                    int asciiIndex = grayValue * (AsciiChars.Length - 1) / 255;
                    asciiArt.Append(AsciiChars[asciiIndex]);
                }
                asciiArt.AppendLine();
            }

            return asciiArt.ToString();
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
    }
}
