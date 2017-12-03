using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Configuration;


namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class SharesNotesViewModel : BaseViewModel
    {
        private ServiceClient _client;
        private User _curUser;

        /// <summary>
        /// Заметки
        /// </summary>
        public ObservableCollection<NoteModel> _sharesNotes;
        public ObservableCollection<NoteModel> SharesNotes
        {
            get => _sharesNotes;
            set
            {
                _sharesNotes = value;
                NotifyPropertyChanged("SharesNotes");
            }
        }

        /// <summary>
        /// Выбранная заметка
        /// </summary>
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

        public SharesNotesViewModel(User curUser)
        {
            _curUser = curUser;
            _client = _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
        }

        /// <summary>
        /// Открыть инфу о замтке
        /// </summary>
        public ICommand OpenNoteInfoCommand
        {
            get => new BaseCommand((object par)=>
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
        /// Отписаться от заметки
        /// </summary>
        public ICommand UnSubscribeCommand
        {
            get => new BaseCommand((object par)=>
            {
                _client.UnShareNoteToUser(_curUser._id, SelectedNote._id);
                _sharesNotes.Remove(SelectedNote);
            });
        }

        /// <summary>
        /// Загрузить доступные заметки
        /// </summary>
        public ICommand LoadSharesNotesCommand
        {
            get => new BaseCommand((object par) =>
            {
                if (SharesNotes == null)
                {
                    SharesNotes = new ObservableCollection<NoteModel>(_client.GetSharesNotes(_curUser._id));
                    foreach (var k in _sharesNotes) k.LoginAutor = _client.GetBasicUserInfo(k._creator)._login;
                }
            });
        }

        public ICommand ReLoadSharesNotesCommand
        {
            get => new BaseCommand((object par) =>
            {
                SharesNotes = null;
                LoadSharesNotesCommand.Execute(null);
            });
        }
    }
}