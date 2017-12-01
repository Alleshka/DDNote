using System;
using DigDesNote.UI.WPF2Binding.Code;
using System.Windows.Controls;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Model;
using System.Windows.Input;
using System.Configuration;
using System.Collections.ObjectModel;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class NoteFromCategoryView : BaseViewModel
    {
        private Category _curCat;

        private ObservableCollection<NoteModel> _notes;
        public ObservableCollection<NoteModel> NotesFromCategory
        {
            get => _notes;
            set
            {
                _notes = value;
                NotifyPropertyChanged("NotesFromCategory");
            }
        }

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

        public NoteFromCategoryView(Category curCat)
        {
            ServiceClient _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
            _curCat = curCat;

            NotesFromCategory = new ObservableCollection<NoteModel>(_client.GetNotesFromCategory(_curCat._id));
        }

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
    }
}