using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private KMP kmp;
        private LCS lcs;
        private Foto[] images;
        private Biodata[] profiles;
        private Db db;
        private ImageToAsciiConverter _converter;
        private Foto ans;
        private string ansPath;
        
        public MainLogic() { 
            bm = new BM();
            kmp = new KMP(); 
            lcs = new LCS();
            db = new Db();
            db.ProcessImages();
            db.ProcessBiodata();
            Debug.WriteLine(db.GetFotos().Length);
            images = db.GetFotos();
            profiles = db.GetBiodatas();
            foreach (var biodata in profiles)
            {
                Debug.WriteLine(biodata.ToString());
            }

            foreach (var foto in images)
            {
                Debug.WriteLine(foto.ToString());
            }
            _converter = new ImageToAsciiConverter();
            ans = new Foto();
            
        }
        public void  SolveMethod(Bitmap image, string type) {
            string asciiImage = _converter.ConvertImageToAscii(image, 100);
            
        
            bool isMatch = false;
            if (type == "BM")
            {
                for (int i = 0; i < images.Length; i++)
                {
                    Debug.WriteLine("entering");
                    int persen = bm.BoyerMooreSearch(images[i].getAscii(), asciiImage);
                    Debug.WriteLine(persen);


                    if (persen != -1)
                    {
                        Debug.WriteLine("succes");
                        ans = images[i];
                        ansPath = images[i].getPath();
                        isMatch = true;
                        break;
                    }

                }
            }
            else
            {
                for (int i = 0; i < images.Length; i++)
                {
                    Debug.WriteLine("entering");
                    int persen = kmp.KMPSearch(images[i].getAscii(), asciiImage);
                    Debug.WriteLine(persen);


                    if (persen != -1)
                    {
                        Debug.WriteLine("succes");
                        ans = images[i];
                        ansPath = images[i].getPath();
                        isMatch = true;
                        break;
                    }

                }
            }
               
            //if pattern matched

            if (!isMatch)
            {
                Debug.WriteLine("ther is no 100%");
                int commonSubseq = 0;
                for (int i = 0; i < images.Length; i++) {
                    int temp = lcs._lcs(asciiImage, images[i].getAscii(), asciiImage.Length, images[i].getAscii().Length);
                    Debug.WriteLine(temp);
                    if (commonSubseq < temp)
                    {
                        commonSubseq = temp;
                        ans = images[i];
                        Debug.WriteLine(commonSubseq / ans.getAscii().Length);
                    }
           
                }
                
                double percentage  = commonSubseq/ (double)ans.getAscii().Length;
                Debug.WriteLine(percentage);
                if (percentage > 0.7)
                {
                    ansPath = ans.getPath();

                }
                else {
                    //there is no solution
                    Debug.WriteLine("Not 100%");
                }
            }
            else
            {
                //solution is
                Debug.WriteLine("Solution 100% matched");
                Debug.WriteLine(ans.getPath());
                ansPath= ans.getPath();
            
            }
            
        }

        /*
        public void SolveKMP(Bitmap image)
        {
            string asciiImage = _converter.ConvertImageToAscii(image, 100);


            bool isMatch = false;
            for (int i = 0; i < images.Length; i++)
            {

                Debug.WriteLine("entering");
                int persen = kmp.KMPSearch(images[i].getAscii(), asciiImage);
                Debug.WriteLine(persen);
                if (persen != -1)
                {
                    Debug.WriteLine("succes");
                    ans = images[i];
                    ansPath = images[i].getPath();
                    isMatch = true;
                    break;
                }

            }
            //if pattern matched

            if (!isMatch)
            {
                Debug.WriteLine("ther is no 100%");
                int commonSubseq = 0;
                for (int i = 0; i < images.Length; i++)
                {
                    int temp = lcs._lcs(asciiImage, images[i].getAscii(), asciiImage.Length, images[i].getAscii().Length);
                    Debug.WriteLine(temp);
                    if (commonSubseq < temp)
                    {
                        commonSubseq = temp;
                        ans = images[i];
                        Debug.WriteLine(commonSubseq / ans.getAscii().Length);
                    }

                }

                double percentage = commonSubseq / (double)ans.getAscii().Length;
                Debug.WriteLine(percentage);
                if (percentage > 0.7)
                {
                    ansPath = ans.getPath();

                }
                else
                {
                    //there is no solution
                    Debug.WriteLine("Not 100%");
                }
            }
            else
            {
                //solution is
                Debug.WriteLine("Solution 100% matched");
                Debug.WriteLine(ans.getPath());
                ansPath = ans.getPath();

            }

        }
        */

        public string getPath(){

            return ansPath;
        }
    }
}
