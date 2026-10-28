using System.Windows;

namespace AutoSalonApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtLogin.Text) || string.IsNullOrEmpty(TxtPassword.Password))
            {
                MessageBox.Show("Введите логин и пароль", "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = AuthService.Login(TxtLogin.Text.Trim(), TxtPassword.Password);
                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль", "Автосалон",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                new MainWindow(user).Show();
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка подключения: " + ex.Message, "Автосалон",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }
    }
}