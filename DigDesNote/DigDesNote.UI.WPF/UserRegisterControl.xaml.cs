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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
