using System;
using System.Windows.Controls;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для UserRegisterControl.xaml
    /// </summary>
    public partial class UserRegisterControl : UserControl
    {
        public UserRegisterControl()
        {
            InitializeComponent();
        }

        public String Login => _login.Text;
        public String Email => _email.Text;
        public String Password => _password.Password.GetHashCode().ToString();
    }
}
