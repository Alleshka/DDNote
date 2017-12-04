using System;
using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using System.Configuration;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    /// <summary>
    /// Вьюха главной страницы
    /// </summary>
    class MainWindowModel : BaseViewModel
    {
        /// <summary>
        /// Текущий пользователей
        /// </summary>
        public User _curUser = null;

        // Просмотр пользователя
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

        // Личные заметки
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

        // Расшаренные заметки
        private SharesNotesViewModel _sharesNotes;
        public SharesNotesViewModel SharesNotesView
        {
            get => _sharesNotes;
            set
            {
                _sharesNotes = value;
                NotifyPropertyChanged("SharesNotesView");
            }
        }

        // Категории пользователя
        private CategoryViewModel _categories;
        public CategoryViewModel Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                NotifyPropertyChanged("Categories");
            }
        }

       /// <summary>
       /// Открытие формы входа
       /// </summary>
        public ICommand CreateLoginWindow
        {
            get => new BaseCommand((object parametres) =>
            {
                System.Windows.Window mainWindow = parametres as System.Windows.Window;

                var child = new LoginViewModel()
                {
                    Title = "Вход"
                };

                ShowDialog(child);
            });
        }

        /// <summary>
        /// Закрытие программы
        /// </summary>
        public ICommand ClosedProgramm
        {
            get => new BaseCommand((object parametress) =>
            {
                // Смотрим стояло ли запоминание пользователя
                bool rememb = Convert.ToBoolean(ConfigurationManager.AppSettings["remember"]);

                // Если нет, то очищаем данные
                if (!rememb)
                {
                    System.Configuration.Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    currentConfig.AppSettings.Settings["lastid"].Value = new Guid().ToString();
                    currentConfig.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
            });
        }

        /// <summary>
        /// Выход из УЗ
        /// </summary>
        public ICommand LogOut
        {
            get => new BaseCommand((object par) =>
            {
                // Очищаем все данные
                System.Configuration.Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                currentConfig.AppSettings.Settings["remember"].Value = false.ToString();
                currentConfig.AppSettings.Settings["lastid"].Value = new Guid().ToString();
                currentConfig.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // Очищаем заметки
                PersonalNotesView = new PersonalNotesViewModel(null);
                SharesNotesView = new SharesNotesViewModel(null);
                Categories = new CategoryViewModel(null);
                UserViewModel = new UserViewModel(null);

                CreateLoginWindow.Execute(null);

                Guid.TryParse(ConfigurationManager.AppSettings["lastid"], out Guid curId);
                if (Guid.Empty != curId) ReadAllData(curId);
            });
        }

        /// <summary>
        /// Открыть окно создания заметки
        /// </summary>
        public ICommand CreateNoteCommand
        {
            get => new BaseCommand((object par) =>
            {
                if (_personalNotes.PersonalNotes == null)
                    _personalNotes.LoadPersonalNotesCommand.Execute(null);
                var note = new CreateNoteViewModel(_curUser, _personalNotes.PersonalNotes)
                {
                    Title = "Создать заметку"
                };
                ShowDialog(note);
            });
        }

        public void ReadAllData(Guid curId)
        {
            ServiceClient _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
            _curUser = _client.GetBasicUserInfo(curId);

            /// Загружаем инфу о пользователе
            UserViewModel = new UserViewModel(_curUser);

            PersonalNotesView = new PersonalNotesViewModel(_curUser);
            SharesNotesView = new SharesNotesViewModel(_curUser);
            Categories = new CategoryViewModel(_curUser);
        }

        public MainWindowModel(System.Windows.Window mainWindow)
        {
            try
            {
                // Читаем последний ID
                Guid.TryParse(ConfigurationManager.AppSettings["lastid"], out Guid curId);

                if (Guid.Empty == curId)
                {
                    CreateLoginWindow.Execute(mainWindow);

                    // Вторая попытка
                    Guid.TryParse(ConfigurationManager.AppSettings["lastid"], out curId);
                    if (Guid.Empty == curId)
                    {
                        mainWindow.Close(); // Если ID == 0
                        return;
                    }
                }

                ReadAllData(curId);
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

                System.Windows.MessageBox.Show("Ошибка входа");
            }
        }
    }
}
