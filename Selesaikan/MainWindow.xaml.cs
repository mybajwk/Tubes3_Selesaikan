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

namespace Selesaikan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database.Database _database;
        public MainWindow()
        {
            InitializeComponent();
            _database = new Database.Database();
            List<SidikJari> dataSidikJari = _database.GetSidikJari();
            Console.WriteLine(dataSidikJari[0].Nama);
        }
    }
}