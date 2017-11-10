using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
                FileWork.SaveDataJson<LoginSet>(set, "adm//loginset.json");

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
