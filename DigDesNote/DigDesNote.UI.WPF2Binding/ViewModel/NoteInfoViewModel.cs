using System;
using System.Windows.Input;
using System.Collections;
using System.Collections.ObjectModel;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.View;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class NoteInfoViewModel : BaseViewModel
    {

        private NoteModel _curNote;
        private NoteModel _curNoteClone;

        ServiceClient _client;

        public NoteInfoViewModel(NoteModel curNote, ServiceClient client)
        {
            _client = client;

            _curNote = curNote;
            _curNoteClone = (NoteModel)curNote.Clone();
        }      

        public NoteModel CurNoteClone
        {
            get => _curNoteClone;
            set
            {
                _curNoteClone = value;
                NotifyPropertyChanged("CurNoteClone");
            }
        }

        public ICommand UpdateNoteCommand
        {
            get => new BaseCommand(SaveNote);
        }
        public void SaveNote(object par)
        {
            CurNoteClone = _client.UpdateNote(CurNoteClone);
            if (_curNote._title != _curNoteClone._title) _curNote._title = _curNoteClone._title;
            if (_curNote._content != _curNoteClone._content) _curNote._content = _curNoteClone._title;
            if (_curNote._updated != _curNoteClone._updated) _curNote._updated = _curNoteClone._updated;
        }

        public ICommand CanselCommand
        {
            get => new BaseCommand(CanselNote);
        }
        public void CanselNote(object par)
        {
            if (_curNote._title != _curNoteClone._title) _curNoteClone._title = _curNote._title;
            if (_curNote._content != _curNoteClone._content) _curNoteClone._content = _curNote._title;
            if (_curNote._updated != _curNoteClone._updated) _curNoteClone._updated = _curNote._updated;
        }

        public ICommand OpenViewCategoryCommand
        {
            get => new BaseCommand(OpenViewCategory);
        }
        public void OpenViewCategory(object par)
        {
            var cat = new AddCategoryViewModel(_curNoteClone, _client);
            ShowDialog(cat);
        }

        public ICommand ViewShareListCommand
        {
            get => new BaseCommand(OpenShareList);
        }

        public void OpenShareList(object par)
        {
            var temp = new SharesListViewModel(_curNote._id, _client);
            Show(temp);
        }
    }
}
