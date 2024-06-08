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
        private string _BiodataNIK;
        private string _BiodataNama;
        private string _BiodataTempat_lahir;
        private string _BiodataTanggal_lahir;
        private string _BiodataJenis_kelamin;
        private string _BiodataGolongan_darah;
        private string _BiodataAlamat;
        private string _BiodataAgama;
        private string _BiodataStatus_perkawinan;
        private string _BiodataPekerjaan;
        private string _BiodataKewarganegaraan;
        


        
        public string BiodataAgama
        {
        get => _BiodataAgama;
            set{
                _BiodataAgama = value;
                OnPropertyChanged();
            } 
        }
        public string BiodataNIK
        {
        get => _BiodataNIK;
            set
            {
                _BiodataNIK = value;
                OnPropertyChanged();
            }
        }
        public string BiodataNama
        {
            get => _BiodataNama;
            set
            {
                _BiodataNama = value;
                OnPropertyChanged();
            }
        }

        public string BiodataTempat_lahir
        {
            get => _BiodataTempat_lahir;
            set
            {
                _BiodataTempat_lahir = value;
                OnPropertyChanged();
            }
        }

        public string BiodataTanggal_lahir
        {
            get => _BiodataTanggal_lahir;
            set
            {
                _BiodataTanggal_lahir = value;
                OnPropertyChanged();
            }
        }

        public string BiodataJenis_kelamin
        {
            get => _BiodataJenis_kelamin;
            set
            {
                _BiodataJenis_kelamin = value;
                OnPropertyChanged();
            }
        }

        public string BiodataGolongan_darah
        {
            get => _BiodataGolongan_darah;
            set
            {
                _BiodataGolongan_darah = value;
                OnPropertyChanged();
            }
        }

        public string BiodataAlamat
        {
            get => _BiodataAlamat;
            set
            {
                _BiodataAlamat = value;
                OnPropertyChanged();
            }
        }

        public string BiodataStatus_perkawinan
        {
            get => _BiodataStatus_perkawinan;
            set
            {
                _BiodataStatus_perkawinan = value;
                OnPropertyChanged();
            }
        }

        public string BiodataPekerjaan
        {
            get => _BiodataPekerjaan;
            set
            {
                _BiodataPekerjaan = value;
                OnPropertyChanged();
            }
        }

        public string BiodataKewarganegaraan
        {
            get => _BiodataKewarganegaraan;
            set
            {
                _BiodataKewarganegaraan = value;
                OnPropertyChanged();
            }
        }
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
        private Visibility _BiodataVisibility = Visibility.Visible;
        public Visibility BiodataVisibility
        {
            get => _BiodataVisibility;
            set { _BiodataVisibility = value;
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
            BiodataVisibility = Visibility.Collapsed;
            //get Biodata
            BiodataNIK = $"NIK:";
            BiodataNama = $"cupi kodok";
            BiodataTempat_lahir = $"jambi";
            BiodataTanggal_lahir = $"999-666-444";
            BiodataJenis_kelamin = $"waria";
            BiodataGolongan_darah = $"ABC";
            BiodataAlamat = $"Lampung gaming";
            BiodataAgama = $"Majusi";
            BiodataStatus_perkawinan = $"4 istri";
            BiodataPekerjaan = $"Pengocok handal";
            BiodataKewarganegaraan = $"Lampung Empire";


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
