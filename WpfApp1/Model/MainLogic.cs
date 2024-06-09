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
        private Foto[] images;
        private Biodata[] profiles;
        public static Db db;
        private ImageToAsciiConverter _converter;
        private Foto ans;
        private Biodata bio;
        private Alay namaasli;
        private string ansPath;
        private double _percentage;
        public MainLogic(Foto[] fotos, Biodata[] bio) { 
            bm = new BM();
            kmp = new KMP(); 
            lcs = new LCS();
            db = new Db();
            images = null;
            profiles = null;
            /*
            foreach (var biodata in profiles)
            {
                Debug.WriteLine(biodata.ToString());
            }

            foreach (var foto in images)
            {
                Debug.WriteLine(foto.ToString());
            }
            */

            _converter = new ImageToAsciiConverter();
            ans = new Foto();
            namaasli = new Alay();
            
        }

        public void SolveMethod(Bitmap image, string type)
        {
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
                Debug.WriteLine("there is no 100%");
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
                _percentage = percentage;
                if (percentage > 0.7)
                {
                    ansPath = ans.getPath();
                    bool istrue = false;
                    //Cari List Biodata yang sesuai
                    for (int i = 0; i < profiles.Length; i++)
                    {
                        Debug.WriteLine("entering");
                        string nama = namaasli.AlayTransform(profiles[i].Nama);
                        int persen = kmp.KMPSearch(ans.getName().ToLower(), nama);
                        Debug.WriteLine(persen);


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
                            string nama = namaasli.AlayTransform(profiles[i].Nama);
                            int lcsNumber = lcs._lcs(ans.getName().ToLower(), nama, ans.getName().ToLower().Length, nama.Length);
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
                            if (persen >= 0.7)
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
                        Debug.WriteLine("Not 100%");
                    }
                }
                else {


                    bio = null;
                }
            }
            else
            {
                //solution is
                Debug.WriteLine("Solution 100% matched");
                Debug.WriteLine(ans.getPath());
                ansPath = ans.getPath();
                _percentage = 1;
                //Cari List Biodata yang sesuai
                bool istrue = false;
                //Cari List Biodata yang sesuai
                for (int i = 0; i < profiles.Length; i++)
                {
                    Debug.WriteLine("entering");
                    string nama = namaasli.AlayTransform(profiles[i].Nama);
                    int persen = kmp.KMPSearch(ans.getName().ToLower(), nama);
                    Debug.WriteLine(persen);


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
                        if (persen >= 0.7)
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
                    Debug.WriteLine("Not 100%");
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
    }
}
