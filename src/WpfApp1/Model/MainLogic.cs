using Org.BouncyCastle.Utilities.Collections;
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
        private ImageToAsciiConverter _converter;
        private Foto ans;
        private Biodata bio;
        private Alay namaasli;
        private string ansPath;
        private double _percentage;
        public MainLogic() { 
            bm = new BM();
            kmp = new KMP(); 
            lcs = new LCS();
            _converter = new ImageToAsciiConverter();
            ans = new Foto();
            namaasli = new Alay();
        }
        public void SolveMethod(Bitmap image, string type,Foto[] images,Biodata[] profiles)
        {
            string[] TImages = _converter.ConvertImageToAsciiArray(image);

            bool isMatch = false;
            if (type == "BM")
            {
                for (int i = 0; i < images.Length; i++)
                {
                    Debug.WriteLine("entering");
                    bool isFound = false;

                    for (int j = 0; j < TImages.Length; j++)
                    {
                        //Debug.WriteLine(TImages[j]);
                        int persen = bm.BoyerMooreSearch(images[i].getAscii(), TImages[j]);
                        //Debug.WriteLine(persen);
                        if (persen != -1)
                        {
                            Debug.WriteLine("succes");
                            ans = images[i];
                            ansPath = images[i].getPath();
                            isMatch = true;
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound) { break; }
                }
            }
            else
            {
                for (int i = 0; i < images.Length; i++)
                {
                    //Debug.WriteLine("entering");
                    bool isFound = false;

                    for (int j = 0; j < TImages.Length; j++)
                    {
                        //Debug.WriteLine(TImages[j]);
                        int persen = kmp.KMPSearch(images[i].getAscii(),TImages[j]);
                        //Debug.WriteLine(persen);
                        if (persen != -1)
                        {
                            Debug.WriteLine("succes");
                            ans = images[i];
                            ansPath = images[i].getPath();
                            isMatch = true;
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound) { break; }




                }
            }

            //if pattern not matched
            if (!isMatch)
            {
                //Debug.WriteLine("there is no 100%");
                int commonSubseq = 0;
                int markIndex = 0;
                for (int i = 0; i < images.Length; i++)
                {

                    for (int j = 0; j < TImages.Length; j++)
                    {
                        int temp = lcs._lcs(TImages[j], images[i].getAscii(), TImages[j].Length, images[i].getAscii().Length);
                       // Debug.WriteLine($"LCS Timages[{j}]: ",temp.ToString());
                        if (commonSubseq < temp)
                        {
                            markIndex = j;
                            commonSubseq = temp;
                            ans = images[i];
                           // Debug.WriteLine(commonSubseq / ans.getAscii().Length);
                        }
                    }
                }

                double percentage = commonSubseq / (double) TImages[markIndex].Length;
                //Debug.WriteLine($"Percentage Hasil LCS maksimal:",percentage.ToString());
                _percentage = percentage;
                if (percentage > 0.85)
                {
                    ansPath = ans.getPath();
                    bool istrue = false;
                    //Cari List Biodata yang sesuai
                    for (int i = 0; i < profiles.Length; i++)
                    {
                        //Debug.WriteLine("entering");
                        string nama = namaasli.AlayTransform(profiles[i].Nama);
                        int persen = kmp.KMPSearch(ans.getName().ToLower(), nama);
                        //Debug.WriteLine(persen);


                        if (persen != -1)
                        {
                            istrue = true;
                            
                            bio = profiles[i];
                            
                            break;
                        }
                    }

                    if (!istrue)
                    {
                        int common = 0;
                        for (int i = 0; i < profiles.Length; i++)
                        {
                            
                            string nama = namaasli.AlayTransform(profiles[i].Nama);
                            int lcsNumber = lcs._lcs(ans.getName().ToLower(), nama, ans.getName().ToLower().Length, nama.Length);
                            
                            if (common < lcsNumber)
                            {
                                common = lcsNumber;
                                bio = profiles[i];
                            }
                        }

                        if (common != 0)
                        {

                            double persen = common / (double)Math.Min(bio.Nama.Length, ans.getName().Length);
                            //Debug.WriteLine(persen);
                            if (persen >= 0.65)
                            {
                                return;
                            }
                            else
                            {
                                //No Solution <0.65
                                bio = null;


                            }

                        }
                        else
                        {

                            //No Solution
                            bio = null;
                        }
                    }
                    else
                    {
                        //there is no solution
                        bio = null;
                        //Debug.WriteLine("Not 100%");
                    }
                }
                else {


                    bio = null;
                }
            }
            else
            {
                //solution is

                ansPath = ans.getPath();
                _percentage = 1;
                //Cari List Biodata yang sesuai
                bool istrue = false;
                //Cari List Biodata yang sesuai
                for (int i = 0; i < profiles.Length; i++)
                {
                    
                    string nama = namaasli.AlayTransform(profiles[i].Nama);
                    
                    int persen = kmp.KMPSearch(ans.getName().ToLower(), nama);
                   


                    if (persen != -1)
                    {
                        istrue = true;
                        Debug.WriteLine("succes");
                        bio = profiles[i];
                        Debug.WriteLine(bio);
                        break;
                    }
                }

                if (!istrue)
                {
                    int common = 0;
                    for (int i = 0; i < profiles.Length; i++)
                    {
                        Debug.WriteLine("entering");
                        string name = namaasli.AlayTransform(profiles[i].Nama);
                        int lcsNumber = lcs._lcs(ans.getName().ToLower(), name, ans.getName().ToLower().Length, name.Length);
                        Debug.WriteLine(lcsNumber);
                        if (common < lcsNumber)
                        {
                            common = lcsNumber;
                            bio = profiles[i];
                        }
                    }

                    if (common != 0)
                    {

                        double persen = common / (double)Math.Min(bio.Nama.Length, ans.getName().Length);
                        Debug.WriteLine(persen);
                        if (persen >= 0.65)
                        {
                            return;
                        }
                        else
                        {
                            //No Solution < 0.7 
                            bio = null;


                        }

                    }
                    else
                    {

                        //No Solution
                        bio = null;



                    }
                }
                else
                {
                    //there is no solution
                    bio = null;
                   // Debug.WriteLine("Not 100%");
                }
            }
        }

        public double getPercentage()
        {
            
            _percentage *= 100;
            _percentage = Math.Round(_percentage, 2);
            return _percentage;
        }
        public string getPath(){

            return ansPath;
        }
        public Biodata getBio() {

            return bio;
        }
        public string nama()
        {
            return ans.getName();
        }
    }
}
