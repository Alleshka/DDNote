using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;
using System.Configuration;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    /// <summary>
    /// Вьюха окна с заметкой
    /// </summary>
    public class NoteInfoViewModel : BaseViewModel
    {
        ServiceClient _client;

        // Заметки
        private NoteModel _curNote;
        private NoteModel _curNoteClone;
        public NoteModel CurNoteClone
        {
            get => _curNoteClone;
            set
            {
                _curNoteClone = value;
                NotifyPropertyChanged("CurNoteClone");
            }
        }

        public NoteInfoViewModel(NoteModel curNote)
        {
            _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
            _curNote = curNote;
            _curNoteClone = (NoteModel)curNote.Clone();
            _curNote.LoginAutor = _client.GetBasicUserInfo(curNote._creator)._login;
            _curNoteClone.LoginAutor = _curNote.LoginAutor;
        }

        /// <summary>
        /// Сохранение заметки
        /// </summary>
        public ICommand UpdateNoteCommand
        {
            get => new BaseCommand((object par) =>
            {
                CurNoteClone = _client.UpdateNote(CurNoteClone);
                _curNoteClone.LoginAutor = _curNote.LoginAutor;
                if (_curNote._title != _curNoteClone._title) _curNote._title = _curNoteClone._title;
                if (_curNote._content != _curNoteClone._content) _curNote._content = _curNoteClone._title;
                if (_curNote._updated != _curNoteClone._updated) _curNote._updated = _curNoteClone._updated;
            });
        }

        /// <summary>
        /// Отмена изменения
        /// </summary>
        public ICommand CanselCommand
        {
            get => new BaseCommand((object par) =>
            {
                if (_curNote._title != _curNoteClone._title) _curNoteClone._title = _curNote._title;
                if (_curNote._content != _curNoteClone._content) _curNoteClone._content = _curNote._title;
                if (_curNote._updated != _curNoteClone._updated) _curNoteClone._updated = _curNote._updated;
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
                    Title = "Категории"
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
                ServiceClient _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
                NoteModel note = par as NoteModel;

                var temp = new SharesListViewModel(note)
                {
                    Title = "Доступно пользователям"
                };

                Show(temp);
            });
        }

        public ICommand ViewCreatorInfoCommand
        {
            get => new BaseCommand((object par) =>
            {
                ServiceClient _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
                User user = _client.GetBasicUserInfo(_curNoteClone._creator);
                var tmp = new UserViewModel(user)
                {
                    Title = $"Информация о пользователе {user._login}"
                };
                ShowDialog(tmp);
            });
        }
    }
}
