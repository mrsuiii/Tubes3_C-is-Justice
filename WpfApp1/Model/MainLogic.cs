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
        private Foto[] images;
        private Db db;
        private ImageToAsciiConverter _converter;
        private Foto ans;
        private string ansPath;
        
        public MainLogic() { 
            bm = new BM();
            km = new KMP(); 
            lcs = new LCS();
            db = new Db();
            images = new Foto[db.GetFotos().Length];
            _converter= new ImageToAsciiConverter();
        
        }
        public void  SolveBM(Bitmap image) {
            string asciiImage = _converter.ConvertImageToAscii(image, 30);
            
            
            bool isMatch = false;
            for (int i = 0; i < images.Length; i++) {

                
                if (bm.BoyerMooreSearch(asciiImage, images[i].getAscii()) != -1)
                {
                    ans = images[i];   
                    ansPath= images[i].getPath();
                    isMatch = true;
                    break;
                }
                
            }
            //if pattern matched

            if (!isMatch)
            {
                int commonSubseq = 0;
                for (int i = 0; i < images.Length; i++) {
                    int temp = lcs._lcs(asciiImage, images[i].getAscii(), asciiImage.Length, images[i].getAscii().Length);
                    if (commonSubseq < temp)
                    {
                        commonSubseq = temp;
                        ans = images[i];
                    }
           
                }
                
                double percentage  = commonSubseq/ (double)ans.getAscii().Length;
                if (percentage > 0.7)
                {
                    ansPath = ans.getPath();

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
