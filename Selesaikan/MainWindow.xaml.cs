using System.Text;
using System.Windows;
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
        public MainWindow()
        {
            InitializeComponent();
            SetInitialImage();
            this.currentActiveAlgorithm = "KMP";
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
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";

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

    }
}