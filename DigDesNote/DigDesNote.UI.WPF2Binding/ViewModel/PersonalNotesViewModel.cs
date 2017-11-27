using System;
using System.Collections.Generic;
using System.Linq;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;


namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class PersonalNotesViewModel : BaseViewModel
    {
        private ObservableCollection<NoteModel> _personalNotes;
        private ServiceClient _client;

        public PersonalNotesViewModel(Guid id, ServiceClient client)
        {
            _client = client;
            _personalNotes = new ObservableCollection<NoteModel>(client.GetPersonalNotes(id));
        }

        public ObservableCollection<NoteModel> PersonalNotes
        {
            get => _personalNotes;
            set
            {
                _personalNotes = value;
                NotifyPropertyChanged("PersonalNotes");
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

        public ICommand OpenNoteInfoCommand
        {
            get => new BaseCommand(OpenNoteInfo, null);
        }
        public ICommand CreateNoteCommand
        {
            get => new BaseCommand(CreateNewNote, null);
        }

        private void OpenNoteInfo(object parametress)
        {
            var note = new NoteInfoViewModel(SelectedNote, _client)
            {
                Title = "Информация о заметке"
            };
            ShowDialog(note);
            NotifyPropertyChanged("PersonalNotes");
        }
        private void CreateNewNote(object parametress)
        {
        }
    }
}
