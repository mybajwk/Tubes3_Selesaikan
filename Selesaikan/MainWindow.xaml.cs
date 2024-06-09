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
        private Dictionary<string,string> cacheASCII = new Dictionary<string, string>();

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

                Dictionary<string,Biodata> allBiodata = _database.GetAllBiodata();
                _database.TruncateBiodata();
                _database.SaveAllBiodata(allBiodata);
                loadedConfig.IsEncrypted = true;
                Config.Config.SaveConfiguration(loadedConfig, "config.json");
            }
            Console.WriteLine("Work: "+Directory.GetCurrentDirectory());
        }

        public void UpdateResultBiodata()
        {

            NamaUser.Content = resultSidikJari.Nama;
            NikUser.Content = resultBiodata.Nik;
            TempatLahirUser.Content = resultBiodata.TempatLahir;
            TanggalLahirUser.Content = resultBiodata.TanggalLahir;
            JenisKelaminUser.Content = resultBiodata.JenisKelamin;
            GolonganDarahUser.Content = resultBiodata.GolonganDarah;
            myTextBlock.Text = resultBiodata.Alamat;
            AgamaUser.Content = resultBiodata.Agama;
            StatusPerkawinanUser.Content = resultBiodata.StatusPerkawinan;
            PekerjaanUser.Content = resultBiodata.Pekerjaan;
            KewarganegaraanUser.Content = resultBiodata.Kewarganegaraan;
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
            this.StopStopwatch();
            Console.WriteLine("KNTL");
            double elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

            setSearchTime(elapsedMilliseconds);
            stopwatch.Reset();

            resultSidikJari = sidikJari;
            List<string> alLName = _database.GetAllBiodata().Values.Select(obj => obj.Nama).ToList();

            string nama = Utils.MatchTexts(sidikJari.Nama, alLName);
            if (nama != "")
            {
                Biodata bio = _database.GetBiodata(nama);
                setResultBiodata(bio);
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

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            // Taking image from state from image loading
            if (!hasUserUpload)
            {
                MessageBox.Show("Please upload an image first");
                return;
            }
            this.StartStopwatch();
            
            BitmapImage entryBitmapImage = getEntryImage();
            (double simil, SidikJari result) = FindMatchSidikJari(entryBitmapImage,_database.GetSidikJari(),currentActiveAlgorithm=="KMP");
            if(simil>70){
                setSimilarityPercentage(simil);
                setResultSidikJari(result);
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

        public (double,SidikJari) FindMatchSidikJari(BitmapImage input, List<SidikJari> dataSidikJari, bool isKMP){
            string entryBinaryString = Utils.preproccToBinary(input);

            string entryAscii = Utils.BinaryStringToASCII(entryBinaryString);

            // Taking some blocks that is good for comparing
            string[] goodEntryBinaryString = Utils.GetChoosenBlockBinaryString(entryBinaryString, 32, 8);

            List<String> goodEntryAsciiString = new List<string>();
            foreach (string binaryString in goodEntryBinaryString)
            {
                string goodascii = Utils.BinaryStringToASCII(binaryString);
                Console.WriteLine(goodascii);
                goodEntryAsciiString.Add(goodascii);
            }
            
            List<Tuple<SidikJari, int>> sidikJari_HammingDistance = new List<Tuple<SidikJari, int>>();
            foreach (SidikJari sidikJari in dataSidikJari)
            {
                if (sidikJari.BerkasCitra != null)
                {
                    string ASCIIstr;
                    if(!cacheASCII.TryGetValue(sidikJari.BerkasCitra,out ASCIIstr)){
                        string filePath = Path.GetFullPath(sidikJari.BerkasCitra);
                        Console.WriteLine(filePath);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        bitmap.EndInit();

                        ASCIIstr = Utils.PreproccToASCII(bitmap);
                        cacheASCII.Add(sidikJari.BerkasCitra,ASCIIstr);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        if (isKMP)
                        {
                            if (Kmp.KmpSearch(ASCIIstr, goodEntryAsciiString[i]) != -1)
                            {
                                return(100.00,sidikJari);
                            }
                        }else{
                            
                            if (Bm.Search(ASCIIstr, goodEntryAsciiString[i]) != -1)
                            {
                                return(100.00,sidikJari);
                            }
                        }
                    }

                    try
                    {
                        int hdValue = Hd.Calculate(ASCIIstr, entryAscii);
                        sidikJari_HammingDistance.Add(new Tuple<SidikJari, int>(sidikJari, hdValue));
                    }
                    catch (Exception _e)
                    {
                        Console.WriteLine("someeror in execption");
                    }
                    
                }

            }

            var tupleWithLeastHammingDistance = sidikJari_HammingDistance.MinBy(t => t.Item2);
            return(100*(1 - (tupleWithLeastHammingDistance.Item2 / (input.Width * input.Height))),tupleWithLeastHammingDistance.Item1);
        }
    }
}