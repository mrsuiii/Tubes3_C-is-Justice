using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WpfApp1.Utilities;
using System.Drawing;
namespace WpfApp1.ViewModel
{
    public class KMP
    {

        private readonly ImageToAsciiConverter _converter;
        public KMP() {
            _converter= new ImageToAsciiConverter();
        }

        public void SolveKMP(BitmapImage imageTarget, BitmapImage[] dataImage)
        {
            Bitmap trgt = _converter.BitmapImageToBitmap(imageTarget);
            string _asciiImage = _converter.ConvertImageToAscii(trgt, 30);
            string[] _asciiImages = new string[dataImage.Length];
            for (int i = 0; i < dataImage.Length; i++)
            {
                Bitmap temp = _converter.BitmapImageToBitmap(dataImage[i]);
                _asciiImages[i] = _converter.ConvertImageToAscii(temp, 30);
            }


            
        }


    }
}
