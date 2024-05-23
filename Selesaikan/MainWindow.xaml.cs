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
using System.Drawing;
using Selesaikan.Utility;
using Selesaikan.Algorithm;
using System.IO;
using Path = System.IO.Path;

namespace Selesaikan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentActiveAlgorithm;
        private BitmapImage entryImage;
        private Database.Database _database;
        private SidikJari resultSidikJari;
        private Biodata resultBiodata;
        public MainWindow() 
        {
            InitializeComponent();
            SetInitialImage();
            currentActiveAlgorithm = "KMP";
            _database = new Database.Database();
            UpdateButtonColors();
            resultSidikJari = new SidikJari();
            resultBiodata = new Biodata();
            entryImage = new BitmapImage();
        }

        public void setResultSidikJari(SidikJari sidikJari)
        {
            resultSidikJari = sidikJari;
        }

        public void setResultBiodata(Biodata biodata)
        {
            resultBiodata = biodata;
        }

        public void setEntryImage(BitmapImage newEntryImage) {
            entryImage = newEntryImage;
        }

        public BitmapImage getEntryImage() {
            return entryImage;
        }
        public BitmapImage CreateBitmapImageFromPath(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;  // This option ensures the image is loaded when it's created
            bitmap.EndInit();
            return bitmap;
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
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                bitmap.EndInit();
                displayImage.Source = bitmap;
                setEntryImage(bitmap);
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

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            if (bitmapImage.StreamSource == null)
            {
                throw new ArgumentNullException("StreamSource is null");
            }

            using (MemoryStream outStream = new MemoryStream())
            {
                bitmapImage.StreamSource.CopyTo(outStream);
                outStream.Seek(0, SeekOrigin.Begin); // Reset the stream position to the beginning
                return new Bitmap(outStream);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // Taking image from state from image loading
            BitmapImage entryBitmapImage = getEntryImage();
            Console.WriteLine(entryBitmapImage);

            // Converting bitmap image type to bitmap
            Bitmap entryBitmap = BitmapImage2Bitmap(entryBitmapImage);

            // Converting bitmap type to Grayscale image
            Bitmap entryGrayScaleImage = Utils.ConvertToGrayscale(entryBitmap);

            // Converting grayscale image into binary image
            Bitmap entryBinaryBitmap = Utils.ConvertToBinary(entryGrayScaleImage);

            // Converting binary bitmap to binary string
            string entryBinaryString = Utils.GetBinaryString(entryBinaryBitmap);

            // Taking some blocks that is good for comparing
            string[] goodEntryBinaryString = Utils.GetChoosenBlockBinaryString(entryBinaryString, 32, 8);

            // Change good entry binary string to ascii string
            List<String> goodEntryAsciiString = new List<string>();
            foreach (string binaryString in goodEntryBinaryString)
            {
                goodEntryAsciiString.Add(binaryString);
            }

            // Comparing good ASCII string to database
            List<SidikJari> dataSidikJari = _database.GetSidikJari();
            List<Tuple<SidikJari, int>> sidikJari_HammingDistance = new List<Tuple<SidikJari, int>>();
            bool isMatchFound = false;
            foreach (SidikJari sidikJari in dataSidikJari)
            {
                Console.WriteLine("okok", goodEntryAsciiString[0]);
                if (sidikJari.BerkasCitra != null)
                {
                    string filePath = Path.GetFullPath("../../../"+sidikJari.BerkasCitra);
                   Console.WriteLine(filePath);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    bitmap.EndInit();
                    Bitmap tempEntryBitmap = BitmapImage2Bitmap(bitmap);

                    // Converting bitmap type to Grayscale image
                    Bitmap imageSidikJariGrayscale = Utils.ConvertToGrayscale(tempEntryBitmap);

                    // Converting grayscale image into binary image
                    Bitmap imageSidikJariBinary = Utils.ConvertToBinary(imageSidikJariGrayscale);

                    // Converting binary bitmap to binary string
                    string imageSidikJariBinaryString = Utils.GetBinaryString(imageSidikJariBinary);

                    // Converting binary strng into ascii string
                    string imageSidikJariAscii = Utils.BinaryStringToASCII(imageSidikJariBinaryString);

                    for (int i = 0; i < 5; i++)
                    {
                        // If the pattern matching found
                        if (currentActiveAlgorithm == "KMP")
                        {
                            if (Kmp.KmpSearch(imageSidikJariAscii, goodEntryAsciiString[i]) != -1)
                            {
                                isMatchFound = true;
                            }
                        } else if (currentActiveAlgorithm == "BM") {
                            if (Bm.Search(imageSidikJariAscii, goodEntryAsciiString[i]) != -1)
                            {
                                isMatchFound = true;
                            }
                        }
                    }
                    if (isMatchFound)
                    {
                        setResultSidikJari(sidikJari);
                        // set biodata juga
                        break;
                    }
                    // else
                    // {
                    //     int leastHammingDistance = 9999;
                    //
                    //     // Finding the least hamming distance
                    //     for (int i = 0; i < 5; i++)
                    //     {
                    //         int hdValue = Hd.Calculate(imageSidikJariAscii, goodEntryAsciiString[i]);
                    //         if (hdValue < leastHammingDistance)
                    //         {
                    //             leastHammingDistance = hdValue;
                    //         }
                    //     }
                    //
                    //     sidikJari_HammingDistance.Add(new Tuple<SidikJari, int>(sidikJari, leastHammingDistance));
                    // }
                }
            }
            if (!isMatchFound)
            {
                // find tuple with least hamming distance
                var tupleWithLeastHammingDistance = sidikJari_HammingDistance.MinBy(t => t.Item2);

                if (tupleWithLeastHammingDistance != null)
                {
                    setResultSidikJari(tupleWithLeastHammingDistance.Item1);
                }
            }
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
                    ((System.Windows.Controls.Image)sender).Source = new BitmapImage(new Uri(file));
                    setEntryImage(new BitmapImage(new Uri(file)));
                }
                else
                {
                    MessageBox.Show("Please drop a BMP file.");
                }
            }
        }
    }
}