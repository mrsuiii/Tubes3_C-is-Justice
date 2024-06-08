using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp1.Utilities;
using System.Drawing;
using WpfApp1.Model;
using System.IO;

namespace WpfApp1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _fingerPrintImage;
        private Bitmap bitmapFingerPrintImage;
        private BitmapImage _solutionImage;
        private ImageToAsciiConverter _converter;
        private string typeAlgorithm;
        private MainLogic _solver;
        private string _searchTime;
        private string _matchPercentage;
        public string SearchTime
        {
            get => _searchTime;
            set
            {
                _searchTime = value;
                OnPropertyChanged();
            }
        }

        public string MatchPercentage
        {
            get => _matchPercentage;
            set
            {
                _matchPercentage = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage FingerPrintImage
        {
            get => _fingerPrintImage;
            set
            {
                _fingerPrintImage = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage SolutionImage
        {
            get => _solutionImage;
            set
            {
                _solutionImage = value;
                OnPropertyChanged();
            }
        }

        private Visibility _fingerPrintInputTextVisibility = Visibility.Visible;
        public Visibility FingerPrintInputTextVisibility
        {
            get => _fingerPrintInputTextVisibility;
            set
            {
                _fingerPrintInputTextVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility _ImageSolutionTextVisibility = Visibility.Visible;
        public Visibility ImageSolutionTextVisibility
        {
            get => _ImageSolutionTextVisibility;
            set
            {
                _ImageSolutionTextVisibility= value;
                OnPropertyChanged();
            }
        }

        public ICommand UploadImageCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleAlgoritmaCommand { get; }
        


        public MainWindowViewModel()
        {
            UploadImageCommand = new RelayCommand(_ => UploadImage());
            SearchCommand = new RelayCommand(_ => Search());
            ToggleAlgoritmaCommand = new RelayCommand(param => ToggleAlgoritma((bool)param));
            _converter = new ImageToAsciiConverter(); 
            _solver = new MainLogic();
                    
        }
        public void UploadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BMP files (*.bmp) | *.bmp"; // Only accept BMP files
            if (openFileDialog.ShowDialog() == true)
            {
                FingerPrintImage = new BitmapImage(new Uri(openFileDialog.FileName));
                FingerPrintInputTextVisibility = Visibility.Collapsed;
                bitmapFingerPrintImage = _converter.BitmapImageToBitmap(FingerPrintImage);
            }
        }

        private BitmapImage LoadBitmapImage(string imagePath)
        {
            BitmapImage bitmapImage = new BitmapImage();

            try
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error loading image: " + ex.Message);
            }

            return bitmapImage;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Search()
        {
            string basePath =  AppDomain.CurrentDomain.BaseDirectory;

            Stopwatch stopwatch= new Stopwatch();

            /*
            if (typeAlgorithm == "BM")
            {
                //solveBM()
                Debug.WriteLine("BM");
                stopwatch.Start();
                _solver.SolveMethod(bitmapFingerPrintImage, "BM");
                stopwatch.Stop();            
            }
            else {
                //solveKMP()
                Debug.WriteLine("KMP");
                stopwatch.Start();
                _solver.SolveMethod(bitmapFingerPrintImage, "KMP");
                stopwatch.Stop();
            }
            */

            Debug.WriteLine(typeAlgorithm);
            stopwatch.Start();
            _solver.SolveMethod(bitmapFingerPrintImage, typeAlgorithm);
            stopwatch.Stop();

            string matchImage = _solver.getPath();

            if (matchImage != null)
            {
                matchImage = Path.Combine(basePath, matchImage);
                SolutionImage = LoadBitmapImage(matchImage);
            }
            //


            SearchTime = $"{stopwatch.ElapsedMilliseconds} ms";
            MatchPercentage = $"98.76%"; // Contoh nilai, ganti dengan nilai yang sesuai

            ImageSolutionTextVisibility = Visibility.Collapsed;
        }
        public void ToggleAlgoritma(bool value)
        {
            if (value)
            {
                typeAlgorithm = "BM";
            }
            else
            {
                typeAlgorithm = "KMP";
            }
        }
    }
}
