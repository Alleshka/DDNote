using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using System.Windows.Controls;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private ServiceClient client;

        private String _login;
        private bool _status;

        public LoginViewModel()
        {
            client = new ServiceClient("http://localhost:41606/api/");
            _login = "Alleshka";
        }

        public String Login
        {
            get => _login;
            set
            {
                _login = value;
                NotifyPropertyChanged("Login");
            }
        }
        public bool Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public ICommand CloseCommand
        {
            get => new BaseCommand(Close, null);
        }
        public ICommand LoginCommand
        {
            get => new BaseCommand(LoginAction);
        }
        public ICommand RegisterCommand
        {
            get => new BaseCommand(OpenRegister);
        }

        private void LoginAction(object parametres)
        {
            try
            {
                PasswordBox box = parametres as PasswordBox;
                User user = new User()
                {
                    _login = this._login,
                    _pass = box.Password
                };

                Guid id = client.Login(user);
                Close(null);
                //CloseCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void OpenRegister(object parametres)
        {
            var register = new RegisterViewModel()
            {
                Title = "Регистрация"
            };
            Show(register);
        }
    }
}
