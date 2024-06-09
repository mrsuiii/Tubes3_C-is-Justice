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
        private string _BiodataMatch;
        private string _fingerPrintMatch;
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
        private Biodata _BiodataSolution;
        private Foto[] foto;
        private Biodata[] bio;

        public string BiodataAgama
        {
            get => _BiodataAgama;
            set {
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
        public string FingerPrintMatch
        {
            get => _fingerPrintMatch;
            set
            {
                _fingerPrintMatch = value;
                OnPropertyChanged();

            }

        }
        public string BiodataMatch {
            get => _BiodataMatch;
            set {
                _BiodataMatch = value;
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

        private Visibility _BiodataVisibility;
        public Visibility BiodataVisibility
        {
            get=> _BiodataVisibility;
            set
            {
                _BiodataVisibility = value;
                OnPropertyChanged();
            }
        }
        private Visibility _BiodataListVisibility = Visibility.Visible;
       
        public Visibility BiodataListVisibility
        {
            get => _BiodataListVisibility;
            set { _BiodataListVisibility = value;
                OnPropertyChanged();

            }
        }
        private Visibility _BiodataDownVisibility;
        public Visibility BiodataDownVisibility { 
            get => _BiodataDownVisibility;
            set
            {
                _BiodataDownVisibility = value;
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
        private Visibility _FingerPrintMatchDownVisibility;

        public Visibility FingerPrintMatchDownVisibility
        {
            get => _FingerPrintMatchDownVisibility;
            set {
                _FingerPrintMatchDownVisibility = value;
                OnPropertyChanged();
            
            }
        }
        private Visibility _SolutionVisibility;
        public Visibility SolutionVisibility
        {
            get => _SolutionVisibility;
            set
            {
                _SolutionVisibility = value;
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

        private MainLogic _loader;


        public MainWindowViewModel()
        {
            UploadImageCommand = new RelayCommand(_ => UploadImage());
            SearchCommand = new RelayCommand(_ => 

                
                Search()
                );
            ToggleAlgoritmaCommand = new RelayCommand(param => ToggleAlgoritma((bool)param));
            _converter = new ImageToAsciiConverter();

            Db db = new Db();
            db.InsertImagePathsAndNames();
            db.ProcessImages();
            db.ProcessBiodata();

            foto = db.GetFotos();
            bio = db.GetBiodatas();
                    
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
        public void ClearPreviousResults()
        {
            // Reset properti atau visibilitas yang diperlukan sebelum pencarian
            BiodataVisibility = Visibility.Collapsed;
            SolutionVisibility = Visibility.Collapsed;
            SolutionImage = LoadBitmapImage(null);
            ImageSolutionTextVisibility = Visibility.Visible;
            BiodataNIK = string.Empty;
            BiodataNama = string.Empty;
            BiodataTempat_lahir = string.Empty;
            BiodataTanggal_lahir = string.Empty;
            BiodataJenis_kelamin = string.Empty;
            BiodataGolongan_darah = string.Empty;
            BiodataAlamat = string.Empty;
            BiodataAgama = string.Empty;
            BiodataStatus_perkawinan = string.Empty;
            BiodataPekerjaan = string.Empty;
            BiodataKewarganegaraan = string.Empty;

            BiodataListVisibility = Visibility.Visible;
            FingerPrintMatchDownVisibility = Visibility.Collapsed;
            BiodataDownVisibility = Visibility.Collapsed;

            // Tambahkan properti lain yang perlu di-reset di sini
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
            ClearPreviousResults();
            _solver = new MainLogic(foto, bio);    
            string basePath =  AppDomain.CurrentDomain.BaseDirectory;
            
            //SolutionVisibility = Visibility.Collapsed;
            
            Stopwatch stopwatch= new Stopwatch();
            Debug.WriteLine(typeAlgorithm);
            stopwatch.Start();
            _solver.SolveMethod(bitmapFingerPrintImage, typeAlgorithm);
            stopwatch.Stop();
            _BiodataSolution = _solver.getBio();
            string matchImage = _solver.getPath();
            if (matchImage == null)
            {
                SolutionImage = LoadBitmapImage(matchImage);
                ImageSolutionTextVisibility = Visibility.Collapsed;
                FingerPrintMatch = "Tidak ada sidik jari yang cocok";
                FingerPrintMatchDownVisibility = Visibility.Visible;
                //case 1: fingerprint not match
                SearchTime = $"{stopwatch.ElapsedMilliseconds} ms";
                MatchPercentage = $"{_solver.getPercentage()}%";
            }
            else if (_BiodataSolution != null)
            {
                ImageSolutionTextVisibility = Visibility.Collapsed;
                SolutionVisibility = Visibility.Visible;
                BiodataVisibility = Visibility.Visible;
                matchImage = Path.Combine(basePath, matchImage);
                SolutionImage = LoadBitmapImage(matchImage);
                BiodataListVisibility = Visibility.Collapsed;
                //get and set matchtime and percentage
                SearchTime = $"{stopwatch.ElapsedMilliseconds} ms";
                MatchPercentage = $"{_solver.getPercentage()}%";
                //get Biodata
                BiodataNIK = $"NIK: {_BiodataSolution.NIK}";
                BiodataNama = $"Nama: {_BiodataSolution.Nama} ";
                BiodataTempat_lahir = $"Tempat Lahir: {_BiodataSolution.TempatLahir}";
                BiodataTanggal_lahir = $"Tanggal Lahir: {_BiodataSolution.TanggalLahir}";
                BiodataJenis_kelamin = $"Jenis Kelamin: {_BiodataSolution.JenisKelamin}";
                BiodataGolongan_darah = $"Golongan Darah: {_BiodataSolution.GolonganDarah}";
                BiodataAlamat = $"Alamat: {_BiodataSolution.Alamat}";
                BiodataAgama = $"Agama: {_BiodataSolution.Agama}";
                BiodataStatus_perkawinan = $"Status Perkawinan: {_BiodataSolution.StatusPerkawinan}";
                BiodataPekerjaan = $"Pekerjaan: {_BiodataSolution.Pekerjaan}";
                BiodataKewarganegaraan = $"Kewarganegaraan: {_BiodataSolution.Kewarganegaraan}";

            }
            else
            {
                SolutionVisibility = Visibility.Visible;
                ImageSolutionTextVisibility = Visibility.Collapsed;
                BiodataMatch = "Tidak ada Biodata yang cocok";
                matchImage = Path.Combine(basePath, matchImage);
                SolutionImage = LoadBitmapImage(matchImage);
                SearchTime = $"{stopwatch.ElapsedMilliseconds} ms";
                MatchPercentage = $"{_solver.getPercentage()}%";
                //case 3: biodata not match
                BiodataDownVisibility = Visibility.Visible;

            }
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
