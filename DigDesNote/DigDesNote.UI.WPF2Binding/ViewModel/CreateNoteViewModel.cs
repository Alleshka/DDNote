using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    /// <summary>
    /// Вьюха для создания модели
    /// </summary>
    public class CreateNoteViewModel : BaseViewModel
    {
        /// Заголовок
        private String _title;
        public String TitleNote
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged("TitleNote");
            }
        }

        // Содержимоей
        private String _content;
        public String ContentNote
        {
            get => _content;
            set
            {
                _content = value;
                NotifyPropertyChanged("ContentNote");
            }
        }

        /// Заметки пользователя
        private ObservableCollection<NoteModel> _userNotes;
        private ServiceClient _client;
        private User _user;

        public CreateNoteViewModel(User user, ObservableCollection<NoteModel> usernotes, ServiceClient client)
        {
            _userNotes = usernotes;
            _client = client;
            _user = user;
        }

        /// Создание заметки
        public ICommand CreateNewNoteCommand
        {
            get => new BaseCommand((object parametress) =>
            {
                try
                {
                    NoteModel temp = new NoteModel()
                    {
                        _title = TitleNote,
                        _content = TitleNote,
                        _creator = _user._id
                    };

                    temp = _client.CreateNote(temp);
                    _userNotes.Add(temp);
                    Close(null);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }
    }
}