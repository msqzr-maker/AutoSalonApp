using System.Windows;

namespace AutoSalonApp
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (AuthService.Register(TxtLogin.Text.Trim(), TxtPassword.Password, TxtFullName.Text.Trim(), out var err))
            {
                MessageBox.Show("Регистрация успешна. Войдите в систему.", "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show(err, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}