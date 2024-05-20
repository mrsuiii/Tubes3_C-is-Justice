using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Search(object sender, RoutedEventArgs e) { 
        }

        private void ChooseCitra(object sender, RoutedEventArgs e) { }
        

        private void ToggleButton_Algoritma_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_SelectImage(object sender, RoutedEventArgs e)
        {
            // Handle select image click
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePlaceholder.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }


    }
}
