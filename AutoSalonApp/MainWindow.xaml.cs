using System.Data;
using System.Windows;
using System.Windows.Input;

namespace AutoSalonApp
{
    public partial class MainWindow : Window
    {
        private readonly User _u;
        private bool _salesMode;

        private const string CarsSql = @"
            SELECT c.CarID, c.VIN, b.BrandName, m.ModelName, c.Year, c.Color, c.Price, c.Status
            FROM Car c
            JOIN CarModel m ON m.ModelID = c.ModelID
            JOIN Brand b ON b.BrandID = m.BrandID
            ORDER BY c.CarID";

        private const string SalesSql = @"
            SELECT s.SaleID, c.VIN, cu.FullName AS Customer, e.FullName AS Employee,
                   s.SaleDate, s.FinalPrice
            FROM Sale s
            JOIN Car c ON c.CarID = s.CarID
            JOIN Customer cu ON cu.CustomerID = s.CustomerID
            JOIN Employee e ON e.EmployeeID = s.EmployeeID
            ORDER BY s.SaleID";

        public MainWindow(User u)
        {
            InitializeComponent();
            _u = u;
            TxtUser.Text = $"{_u.FullName} ({_u.Role})";
            BtnSales.IsEnabled = u.IsManager;
            BtnAddSale.IsEnabled = u.IsManager;
            Grid.MouseDoubleClick += Grid_DoubleClick;
            Load(false);
        }

        private void Load(bool sales)
        {
            _salesMode = sales;
            try
            {
                Grid.ItemsSource = Db.Query(sales ? SalesSql : CarsSql).DefaultView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowCars_Click(object sender, RoutedEventArgs e) => Load(false);
        private void ShowSales_Click(object sender, RoutedEventArgs e) => Load(true);

        private void AddSale_Click(object sender, RoutedEventArgs e)
        {
            var w = new SaleEditWindow(null, _u);
            if (w.ShowDialog() == true) Load(false);
        }

        private void Grid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!_salesMode) return;
            if (Grid.SelectedItem is not DataRowView row) return;
            var w = new SaleEditWindow((int)row["SaleID"], _u);
            if (w.ShowDialog() == true) Load(true);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}