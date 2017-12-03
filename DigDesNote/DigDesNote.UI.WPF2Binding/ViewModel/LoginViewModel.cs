using System;
using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using System.Windows.Controls;
using DigDesNote.Model;
using System.Configuration;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    /// <summary>
    /// Вьюха входа пользователей
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {

        private ServiceClient client;

        // Логин
        private String _login;
        public String Login
        {
            get => _login;
            set
            {
                _login = value;
                NotifyPropertyChanged("Login");
            }
        }

        // Запоминать ли 
        private bool _status;
        public bool Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public LoginViewModel()
        {
            client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
        }


        public ICommand CloseCommand
        {
            get => new BaseCommand(Close);
        }

        /// Вход пользователя
        public ICommand LoginCommand
        {
            get => new BaseCommand((object parametres)=>
            {
                try
                {
                    // Достаём пароль
                    PasswordBox box = parametres as PasswordBox;
                    User user = new User()
                    {
                        _login = this._login,
                        _pass = box.Password
                    };
                    // Совершаем вход
                    Guid id = client.Login(user);

                    // Записываем данные входа
                    System.Configuration.Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    currentConfig.AppSettings.Settings["lastid"].Value = id.ToString();
                    currentConfig.AppSettings.Settings["remember"].Value = Status.ToString();
                    currentConfig.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");

                    // Закрываем форму
                    Close(null);
                }
                catch (Exception ex)
                {
                    // Записываем данные входа
                    System.Configuration.Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    currentConfig.AppSettings.Settings["lastid"].Value = new Guid().ToString();
                    currentConfig.AppSettings.Settings["remember"].Value = false.ToString();
                    currentConfig.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }

        /// <summary>
        ///  Открыть окно регистрации
        /// </summary>
        public ICommand RegisterCommand
        {
            get => new BaseCommand((object parametres)=>
            {
                var register = new RegisterViewModel()
                {
                    Title = "Регистрация"
                };
                ShowDialog(register);
            });
        }
    }
}
