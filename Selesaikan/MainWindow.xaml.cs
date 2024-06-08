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
using Selesaikan.Config;
using System.Diagnostics;

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
        private Stopwatch stopwatch;

        private double searchTime;
        private double similarityPercentage;

        private bool hasUserUpload = false;

        public void StartStopwatch()
        {
            DisableButtons(); // Disable buttons
            stopwatch.Start();
        }

        public void StopStopwatch()
        {
            stopwatch.Stop();
            EnableButtons(); // Enable buttons
        }

        private void DisableButtons()
        {
            btnLoadImage.IsEnabled = false;
            kmpButton.IsEnabled = false;
            bmButton.IsEnabled = false;
            searchButton.IsEnabled = false;
        }

        private void EnableButtons()
        {
            btnLoadImage.IsEnabled = true;
            kmpButton.IsEnabled = true;
            bmButton.IsEnabled = true;
            searchButton.IsEnabled = true;
        }

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

            this.stopwatch = new Stopwatch();

            AppConfig loadedConfig = Config.Config.LoadConfiguration("config.json");
            if (!loadedConfig.IsEncrypted)
            {

                List<Biodata> allBiodata = _database.GetAllBiodata();
                _database.TruncateBiodata();
                _database.SaveAllBiodata(allBiodata);
                loadedConfig.IsEncrypted = true;
                Config.Config.SaveConfiguration(loadedConfig, "config.json");
            }
        }

        public void UpdateResultBiodata()
        {
            AppConfig loadedConfig = Config.Config.LoadConfiguration("config.json");
            byte[] key = Encoding.UTF8.GetBytes(loadedConfig.Key);
            Blowfish blowfish = new Blowfish(key);


            NamaUser.Content = resultSidikJari.Nama;
            NikUser.Content = blowfish.Decrypt(resultBiodata.Nik);
            TempatLahirUser.Content = blowfish.Decrypt(resultBiodata.TempatLahir);
            TanggalLahirUser.Content = resultBiodata.TanggalLahir;
            JenisKelaminUser.Content = resultBiodata.JenisKelamin;
            GolonganDarahUser.Content = blowfish.Decrypt(resultBiodata.GolonganDarah);
            AlamatUser.Content = blowfish.Decrypt(resultBiodata.Alamat);
            AgamaUser.Content = blowfish.Decrypt(resultBiodata.Agama);
            StatusPerkawinanUser.Content = resultBiodata.StatusPerkawinan;
            PekerjaanUser.Content = blowfish.Decrypt(resultBiodata.Pekerjaan);
            KewarganegaraanUser.Content = blowfish.Decrypt(resultBiodata.Kewarganegaraan);
        }

        public void UpdatePerformanceResult()
        {
            if (this.searchTime > 1000)
            {
                double searchTimeInSeconds = this.searchTime / 1000.0;
                searchTimeLabel.Content = $"Search time    : {searchTimeInSeconds.ToString("F2")} s";
            }
            else
            {
                searchTimeLabel.Content = $"Search time    : {this.searchTime} ms";
            }

            similarityLabel.Content = $"Similarity Percentage : {this.similarityPercentage.ToString("F2")} %";
        }

        public void UpdateResultSidikJari()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent?.Parent?.FullName;

            if (projectDirectory == null)
            {
                throw new DirectoryNotFoundException("Project directory not found.");
            }

            // Combine the project directory with the relative path
            string relativePath = resultSidikJari.BerkasCitra;
            string fullPath = Path.Combine(projectDirectory, relativePath);

            // Ensure the file exists
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"The file '{fullPath}' does not exist.");
            }

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            outputImage.Source = bitmap;
        }

        public void setSearchTime(double newSearchTime)
        {
            this.searchTime = newSearchTime;
        }

        public void setSimilarityPercentage(double newPercentage)
        {
            this.similarityPercentage = newPercentage;
        }

        public void setResultSidikJari(SidikJari sidikJari)
        {
            stopwatch.Stop();
            double elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

            setSearchTime(elapsedMilliseconds);
            stopwatch.Reset();

            resultSidikJari = sidikJari;
            List<Biodata> allBiodata = _database.GetAllBiodata();
            List<string> alLName = new List<string>();
            AppConfig loadedConfig = Config.Config.LoadConfiguration("config.json");
            byte[] key = Encoding.UTF8.GetBytes(loadedConfig.Key);
            Blowfish blowfish = new Blowfish(key);
            foreach (Biodata bio in allBiodata)
            {
                if (bio.Nama != null)
                {
                    alLName.Add(blowfish.Decrypt(bio.Nama));
                }
            }

            string nama = Utils.MatchTexts(sidikJari.Nama, alLName);
            if (nama != "")
            {
                Console.WriteLine("yey");
                Console.WriteLine(nama);
                Biodata bio = _database.GetBiodata(blowfish.Encrypt(nama));
                setResultBiodata(bio);
                if (bio.Nama != null)
                {
                    Console.WriteLine("yey");
                }
                Console.WriteLine(sidikJari.Nama);
                UpdateResultBiodata();
                UpdateResultSidikJari();
                UpdatePerformanceResult();
            } ;
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.BMP)|*.png;*.jpeg;*.jpg;*.BMP"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var imageStream = File.OpenRead(openFileDialog.FileName);
                MemoryStream memoryStream = new MemoryStream();
                imageStream.CopyTo(memoryStream);
                memoryStream.Position = 0; // Reset position after copy
                imageStream.Close(); // Close the file stream

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = memoryStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                displayImage.Source = bitmap;
                setEntryImage(bitmap);
                hasUserUpload = true;
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
            if (bitmapImage.StreamSource == null || !bitmapImage.StreamSource.CanRead)
            {
                throw new ArgumentException("StreamSource is either null or cannot be read");
            }

            // Copy the stream to keep the original stream intact
            MemoryStream outStream = new MemoryStream();
            bitmapImage.StreamSource.Seek(0, SeekOrigin.Begin); // Ensure we start from the beginning
            bitmapImage.StreamSource.CopyTo(outStream);
            outStream.Seek(0, SeekOrigin.Begin); // Reset the position to the beginning

            // Create bitmap from copied stream
            return new Bitmap(outStream);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // Taking image from state from image loading
            if (!hasUserUpload)
            {
                MessageBox.Show("Please upload an image first");
                return;
            }
            stopwatch.Start();
            
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

            string entryAscii = Utils.BinaryStringToASCII(entryBinaryString);

            // Taking some blocks that is good for comparing
            string[] goodEntryBinaryString = Utils.GetChoosenBlockBinaryString(entryBinaryString, 32, 8);

            // Change good entry binary string to ascii string
            List<String> goodEntryAsciiString = new List<string>();
            foreach (string binaryString in goodEntryBinaryString)
            {
                string goodascii = Utils.BinaryStringToASCII(binaryString);
                goodEntryAsciiString.Add(goodascii);
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

                    for (int i = 0; i < 2; i++)
                    {
                        // If the pattern matching found
                        if (currentActiveAlgorithm == "KMP")
                        {
                            if (Kmp.KmpSearch(imageSidikJariAscii, goodEntryAsciiString[i]) != -1)
                            {
                                Console.WriteLine("ketemuu");
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
                        setSimilarityPercentage(100.00);
                        setResultSidikJari(sidikJari);
                        break;
                    }
                    else
                    {
                        try
                        {
                            int hdValue = Hd.Calculate(imageSidikJariAscii, entryAscii);
                            sidikJari_HammingDistance.Add(new Tuple<SidikJari, int>(sidikJari, hdValue));
                        }
                        catch (Exception _e)
                        {
                            Console.WriteLine("someeror in execption");
                        }
                    }
                }

            }
            if (!isMatchFound)
            {
                // find tuple with least hamming distance
                var tupleWithLeastHammingDistance = sidikJari_HammingDistance.MinBy(t => t.Item2);

                if (tupleWithLeastHammingDistance != null)
                {
                    setSimilarityPercentage(1 - (tupleWithLeastHammingDistance.Item2 / (entryBitmap.Width * entryBitmap.Height)));
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