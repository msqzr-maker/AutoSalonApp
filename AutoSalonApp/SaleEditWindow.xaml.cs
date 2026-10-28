using System.Data;
using System.Linq;
using System.Windows;

namespace AutoSalonApp
{
    public partial class SaleEditWindow : Window
    {
        private readonly int? _id;
        private readonly User _u;

        public SaleEditWindow(int? id, User u)
        {
            InitializeComponent();
            _id = id;
            _u = u;
            Title = id.HasValue ? "Редактирование продажи" : "Новая продажа";
            BtnDelete.Visibility = id.HasValue ? Visibility.Visible : Visibility.Collapsed;
            LoadCombos();
            if (id.HasValue) LoadSale();
        }

        private void LoadCombos()
        {
            try
            {
                CmbCar.ItemsSource = Db.Query(
                    "SELECT CarID, VIN FROM Car WHERE Status = N'В наличии'").DefaultView;
                CmbCustomer.ItemsSource = Db.Query(
                    "SELECT CustomerID, FullName FROM Customer").DefaultView;
                CmbEmployee.ItemsSource = Db.Query(
                    "SELECT EmployeeID, FullName FROM Employee").DefaultView;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSale()
        {
            try
            {
                var dt = Db.Query(
                    "SELECT CarID, CustomerID, EmployeeID, FinalPrice FROM Sale WHERE SaleID=@id",
                    ("@id", _id.Value));
                if (dt.Rows.Count == 0) return;
                var r = dt.Rows[0];
                CmbCar.SelectedValue = r["CarID"];
                CmbCustomer.SelectedValue = r["CustomerID"];
                CmbEmployee.SelectedValue = r["EmployeeID"];
                TxtPrice.Text = ((decimal)r["FinalPrice"]).ToString("F2");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message, "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCar.SelectedValue == null
                || CmbCustomer.SelectedValue == null
                || CmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrice.Text, out var price) || price <= 0)
            {
                MessageBox.Show("Некорректная цена", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var p = new (string, object)[]
                {
                    ("@car", CmbCar.SelectedValue),
                    ("@cust", CmbCustomer.SelectedValue),
                    ("@emp", CmbEmployee.SelectedValue),
                    ("@p", price)
                };

                if (_id.HasValue)
                {
                    var args = p.ToList();
                    args.Add(("@id", _id.Value));
                    Db.Exec("UPDATE Sale SET CarID=@car, CustomerID=@cust, EmployeeID=@emp, " +
                            "FinalPrice=@p WHERE SaleID=@id", args.ToArray());
                }
                else
                {
                    Db.Tx((
                        "INSERT INTO Sale(CarID, CustomerID, EmployeeID, SaleDate, FinalPrice) " +
                        "VALUES(@car, @cust, @emp, CAST(GETDATE() AS DATE), @p); " +
                        "UPDATE Car SET Status = N'Продан' WHERE CarID = @car", p));
                }

                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!_id.HasValue) return;
            if (MessageBox.Show("Удалить продажу?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                var dt = Db.Query("SELECT CarID FROM Sale WHERE SaleID=@id", ("@id", _id.Value));
                if (dt.Rows.Count == 0) return;
                var carId = (int)dt.Rows[0]["CarID"];

                Db.Tx(
                    ("DELETE FROM Sale WHERE SaleID=@id", new[] { ("@id", (object)_id.Value) }),
                    ("UPDATE Car SET Status = N'В наличии' WHERE CarID=@c", new[] { ("@c", (object)carId) }));

                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message, "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}