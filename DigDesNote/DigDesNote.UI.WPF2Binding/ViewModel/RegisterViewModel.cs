using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigDesNote.UI.WPF2Binding.Code;
using System.Windows.Controls;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Command;
using System.Windows.Input;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class RegisterViewModel : BaseViewModel
    {

        private ServiceClient client;
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
            client = new ServiceClient("http://localhost:41606/api/");
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
                client.RegisterUser(user);
                Close(null);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
