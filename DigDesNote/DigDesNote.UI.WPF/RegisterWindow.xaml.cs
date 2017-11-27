using System;
using System.Windows;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly ServiceClient _client;

        public RegisterWindow(ServiceClient client)
        {
            InitializeComponent();
            _client = client;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User user = new User()
                {
                    _login = _userControl.Login,
                    _email = _userControl.Email,
                    _pass = _userControl.Password
                };
                user = _client.CreateUser(user);
                MessageBox.Show("Регистрация прошла успешно. Ваш id = " + user._id);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
