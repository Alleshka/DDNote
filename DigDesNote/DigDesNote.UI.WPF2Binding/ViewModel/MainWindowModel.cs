using System;
using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    class MainWindowModel : BaseViewModel
    {
        private ServiceClient _client;

        private UserViewModel _userViewModel;
        public UserViewModel UserViewModel
        {
            get => _userViewModel;
            set
            {
                _userViewModel = value;
                NotifyPropertyChanged("UserViewModel");
            }
        }

        private PersonalNotesViewModel _personalNotes;
        public PersonalNotesViewModel PersonalNotesView
        {
            get => _personalNotes;
            set
            {
                _personalNotes = value;
                NotifyPropertyChanged("PersonalNotesView");
            }
        }

       
        public ICommand CreateLoginWindow { get; set; }

        public MainWindowModel()
        {
            _client = new ServiceClient("http://localhost:41606/api/");
            _client.StartProgram();

            CreateLoginWindow = new BaseCommand(OpenLoginWindow, null);
            CreateLoginWindow.Execute(null);

            AGLibrary.Files.FileWork.ReadDataJson<Guid>(out Guid curId, "adm//curUser.json");

            UserViewModel = new UserViewModel(_client.GetBasicUserInfo(curId));
            PersonalNotesView = new PersonalNotesViewModel(curId, _client);
        }

        private void OpenLoginWindow(object parametres = null)
        {
            if (!System.IO.File.Exists("adm//curUser.json"))
            {
                var child = new LoginViewModel()
                {
                    Title = "Вход"
                };
                ShowDialog(child);
            }
        }
    }
}
