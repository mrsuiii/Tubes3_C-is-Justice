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

namespace WpfApp1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _fingerPrintImage;
        private Bitmap bitmapFingerPrintImage;
        private readonly ImageToAsciiConverter _converter;
        private string typeAlgorithm;
        public BitmapImage FingerPrintImage
        {
            get => _fingerPrintImage;
            set
            {
                _fingerPrintImage = value;
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
        public ICommand UploadImageCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ToggleAlgoritmaCommand { get; }
        public MainWindowViewModel()
        {
            UploadImageCommand = new RelayCommand(_ => UploadImage());
            SearchCommand = new RelayCommand(_ => Search());
            ToggleAlgoritmaCommand = new RelayCommand(param => ToggleAlgoritma((bool)param));
            _converter = new ImageToAsciiConverter(); 
                    
        }
        public void UploadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                FingerPrintImage = new BitmapImage(new Uri(openFileDialog.FileName));
                FingerPrintInputTextVisibility = Visibility.Collapsed;
                bitmapFingerPrintImage = _converter.BitmapImageToBitmap(FingerPrintImage);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void Search()
        {
            if (typeAlgorithm == "BM")
            {
                //solveBM()
                Debug.WriteLine("BM");

            }
            else {
                //solveKMP()
                Debug.WriteLine("KMP");


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
