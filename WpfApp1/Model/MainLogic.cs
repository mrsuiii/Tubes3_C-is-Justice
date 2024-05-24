using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using WpfApp1.Utilities;

namespace WpfApp1.Model
{
    public class MainLogic
    {
        private BM bm;
        private KMP km;
        private LCS lcs;
        private Bitmap[] images;
        private ImageToAsciiConverter _converter;
        private string ans;

        public MainLogic() { 
            bm = new BM();
            km = new KMP(); 
            lcs = new LCS();
            images = new Bitmap[1000];
            _converter= new ImageToAsciiConverter();
        
        }
        public void  SolveBM(Bitmap image) {
            string asciiImage = _converter.ConvertImageToAscii(image, 30);
            string[] asciiImages = new string[images.Length];
            
            bool isMatch = false;
            for (int i = 0; i < asciiImages.Length; i++) {

                asciiImages[i] = _converter.ConvertImageToAscii(images[i], 30);
                if (bm.BoyerMooreSearch(asciiImage, asciiImages[i]) != -1)
                {   
                    ans= asciiImages[i];    
                    isMatch = true;
                    break;
                }
                
            }
            //if pattern matched

            if (!isMatch)
            {
                int commonSubseq = 0;
                for (int i = 0; i < asciiImages.Length; i++) {
                    int temp = lcs._lcs(asciiImage, asciiImages[i], asciiImage.Length, asciiImages[i].Length);
                    if (commonSubseq < temp)
                    {
                        commonSubseq = temp;
                        ans = asciiImages[i];
                    }
           
                }
                
                double percentage  = commonSubseq/ (double)ans.Length;
                if (percentage > 0.7)
                {

                }
                else {  
                    //there is no solution
                    
                }
            }
            else
            {
                //solution is          
            
            }
            
        }
    }
}
