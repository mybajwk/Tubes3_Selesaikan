using System.Text;
using System.Windows;
using Selesaikan.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Selesaikan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentActiveAlgorithm;
        private Database.Database _database;
        public MainWindow()
        {
            InitializeComponent();
            SetInitialImage();
            this.currentActiveAlgorithm = "KMP";
            _database = new Database.Database();
            List<SidikJari> dataSidikJari = _database.GetSidikJari();
            Console.WriteLine(dataSidikJari[0].Nama);
            UpdateButtonColors();
        }

        private void SetInitialImage()
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("pack://application:,,,/Images/insertimage.png");
            bitmap.CacheOption = BitmapCacheOption.OnLoad;  // Ensures the image is cached at load time
            bitmap.EndInit();
            displayImage.Source = bitmap;
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.BMP)|*.png;*.jpeg;*.jpg;*.BMP";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                displayImage.Source = bitmap;
            }
        }

        private void kmpButton_Click(object sender, RoutedEventArgs e)
        {
            currentActiveAlgorithm = "KMP";
            UpdateButtonColors();
        }

        private void bmButton_Click(object sender, RoutedEventArgs e)
        {
            currentActiveAlgorithm = "BM";
            UpdateButtonColors();
        }
        
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
         // do something
         outputImage.Source = displayImage.Source;
        }

        private void UpdateButtonColors()
        {
            // Reset all button backgrounds to default
            kmpButton.Background = new SolidColorBrush(Colors.Azure);
            bmButton.Background = new SolidColorBrush(Colors.Azure);
            searchButton.Background = new SolidColorBrush(Colors.Azure);

            // Highlight the button matching the current algorithm
            switch (currentActiveAlgorithm)
            {
                case "KMP":
                    kmpButton.Background = new SolidColorBrush(Colors.LightGreen);
                    break;
                case "BM":
                    bmButton.Background = new SolidColorBrush(Colors.LightGreen);
                    break;
                case "Search":
                    searchButton.Background = new SolidColorBrush(Colors.LightGreen);
                    break;
            }
        }
        
        private void Image_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                string file = files[0];
                if (file.ToLower().EndsWith(".bmp"))
                {
                    ((Image)sender).Source = new BitmapImage(new Uri(file));
                }
                else
                {
                    MessageBox.Show("Please drop a BMP file.");
                }
            }
        }
    }
}