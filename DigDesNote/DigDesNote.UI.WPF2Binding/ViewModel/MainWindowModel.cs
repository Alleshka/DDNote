using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.View;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    class MainWindowModel : BaseViewModel
    {
        public ICommand CreateLoginWindow { get; set; }

        public MainWindowModel()
        {
            CreateLoginWindow = new BaseCommand(OpenLoginWindow, null);
            CreateLoginWindow.Execute(null);
        }

        private void OpenLoginWindow(object parametres = null)
        {
            var child = new LoginViewModel()
            {
                Title = "Вход"
            };
            Show(child);
        }
    }
}
