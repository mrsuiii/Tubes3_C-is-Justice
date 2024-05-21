using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _fingerPrintImage;
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
        public ICommand SearchCommand{ get; }

        public ICommand ToggleAlgoritmaCommand{ get; }
        public MainWindowViewModel()
        {
            UploadImageCommand = new RelayCommand(_ => UploadImage());
            SearchCommand = new RelayCommand(_ => Search());
            ToggleAlgoritmaCommand = new RelayCommand(param => ToggleAlgoritma((bool)param));
        }

        public void UploadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                FingerPrintImage = new BitmapImage(new Uri(openFileDialog.FileName));
                FingerPrintInputTextVisibility = Visibility.Collapsed;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Search()
        { 
        
        }
        public bool ToggleAlgoritma(bool value) 
        {
            Debug.WriteLine(value);
            return value;
        
        }
       }
}
