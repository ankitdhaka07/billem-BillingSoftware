using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Item> _availableItems = new()
        {
            new Item { Name = "Notebook", Price = 100 },
            new Item { Name = "Pen", Price = 10 },
            new Item { Name = "Pencil", Price = 5 }
        };

        public MainWindow()
        {
            InitializeComponent();
            ItemComboBox.ItemsSource = _availableItems;
            ItemComboBox.SelectedIndex = 0;
        }

    private void GenerateBill_Click(object sender, RoutedEventArgs e)
    {
            var selectedItem = ItemComboBox.SelectedItem as Item;

            if (selectedItem == null)
            {
                MessageBox.Show("No item selected");
                return;
            }
            var item = new Item() { Name = "block", Price = 23 };
            var billItem = new BillItem { Item = item, Quantity=1 };
            var bill = new Bill() { BillItems = new List<BillItem>{ billItem } };
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"bill.pdf");

            BillPdfGenerator.Generate(bill, filePath);

        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });
    }
}
}