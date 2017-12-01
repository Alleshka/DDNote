using System;
using DigDesNote.UI.WPF2Binding.Code;
using System.Windows.Controls;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Command;
using System.Windows.Input;
using System.Configuration;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class RegisterViewModel : BaseViewModel
    {

        private ServiceClient _client;
        private String _login;
        private String _email;

        public String Login
        {
            get => _login;
            set
            {
                _login = value;
                NotifyPropertyChanged("Login");
            }
        }

        public String Email
        {
            get => _email;
            set
            {
                _email = value;
                NotifyPropertyChanged("Email");
            }
        }

        public RegisterViewModel()
        {
            _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
        }

        public ICommand RegisterCommand
        {
            get => new BaseCommand(Register);
        }

        private void Register(object parametres)
        {
            try
            {
                PasswordBox box = parametres as PasswordBox;
                User user = new User()
                {
                    _login = this._login,
                    _email = this._email,
                    _pass = box.Password
                };
                _client.RegisterUser(user);
                Close(null);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
