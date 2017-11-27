using System;
using System.Windows;
using DigDesNote.Model;
using AGLibrary.Files;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ServiceClient _client;

        public LoginWindow(ServiceClient client)
        {
            InitializeComponent();
            _client = client;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow window = new RegisterWindow(_client);
            window.Show();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Инициализируем логин и пароль
                User temp = new User()
                {
                    _login = _login.Text,
                    _pass = _password.Password.GetHashCode().ToString()
                };

                // Получаем ID (в будущем мб token)
                Guid gui = _client.Login(temp);

                // Меняем настройки
                LoginSet set = new LoginSet()
                {
                    _statusLogin = (bool)_storeLogin.IsChecked,
                    _userId = gui
                };

                
                // Сохраняем настройки
                FileWork.SaveDataJson<LoginSet>(set, System.Configuration.ConfigurationManager.AppSettings["loginset"]);

                MessageBox.Show("Вход выполнен успешно");
                this.DialogResult = true; // Закрываем форму
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
