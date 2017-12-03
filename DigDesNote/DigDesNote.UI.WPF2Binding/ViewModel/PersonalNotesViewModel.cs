using System;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Configuration;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class PersonalNotesViewModel : BaseViewModel
    {
        private ServiceClient _client;
        // Текущий пользователь
        private User _curUser;

        // Заметки пользователя
        private ObservableCollection<NoteModel> _personalNotes;
        public ObservableCollection<NoteModel> PersonalNotes
        {
            get => _personalNotes;
            set
            {
                _personalNotes = value;
                NotifyPropertyChanged("PersonalNotes");
            }
        }

        // Выбранная заметка
        private NoteModel _selectedNote;
        public NoteModel SelectedNote
        {
            get => _selectedNote;
            set
            {
                _selectedNote = value;
                NotifyPropertyChanged("SelectedNote");
            }
        }

        public PersonalNotesViewModel(User curUser)
        {
            _curUser = curUser;
            _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
        }

        /// <summary>
        /// Открыть информацию о заметке
        /// </summary>
        public ICommand OpenNoteInfoCommand
        {
            get => new BaseCommand((object par) =>
            {
                var note = new NoteInfoViewModel(SelectedNote)
                {
                    Title = "Информация о заметке"
                };
                ShowDialog(note);
                NotifyPropertyChanged("PersonalNotes");
            });
        }

        /// <summary>
        /// Открыть окно создания заметки
        /// </summary>
        public ICommand CreateNoteCommand
        {
            get => new BaseCommand((object par)=>
            {
                var note = new CreateNoteViewModel(_curUser, _personalNotes)
                {
                    Title = "Создать заметку"
                };
                ShowDialog(note);
            });
        }

        public ICommand DeleteNoteCommand
        {
            get => new BaseCommand((object par)=>
            {
                try
                {
                    _client.DeleteNote(_selectedNote._id);
                    _personalNotes.Remove(_selectedNote);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }

        /// <summary>
        /// Открыть окно категорий
        /// </summary>
        public ICommand OpenViewCategoryCommand
        {
            get => new BaseCommand((object par) =>
            {
                ServiceClient _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
                NoteModel note = par as NoteModel;
                var cat = new AddCategoryViewModel(note)
                {
                    Title = "Каегории"
                };
                ShowDialog(cat);
            });
        }

        /// <summary>
        /// Открыть окно шар
        /// </summary>
        public ICommand ViewShareListCommand
        {
            get => new BaseCommand((object par) =>
            {
                NoteModel _curNote = par as NoteModel;
                var temp = new SharesListViewModel(_curNote)
                {
                    Title = "Доступно для пользователей"
                };
                Show(temp);
            });
        }

        /// Загрузить персональные заметки
        public ICommand LoadPersonalNotesCommand
        {
            get => new BaseCommand((object par) =>
            {
                if (PersonalNotes == null)
                {
                    PersonalNotes = new ObservableCollection<NoteModel>(_client.GetPersonalNotes(_curUser._id));
                    foreach (var k in PersonalNotes) k.LoginAutor = _curUser._login;
                }
            });
        }

        public ICommand ReLoadPersonalNotesCommand
        {
            get => new BaseCommand((object par) =>
            {
                PersonalNotes = null;
                LoadPersonalNotesCommand.Execute(null);
            });
        }
    }
}
