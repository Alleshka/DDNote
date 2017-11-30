using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.ViewModel;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class AddCategoryViewModel : BaseViewModel
    {
        NoteModel _curNote;

        ObservableCollection<NoteCategoriesModel> _userCategories;
        ObservableCollection<NoteCategoriesModel> _noteCategories;

        ServiceClient _client;
        public AddCategoryViewModel(NoteModel note, ServiceClient client)
        {
            _client = client;
            _curNote = note;

            
            _userCategories = new ObservableCollection<NoteCategoriesModel>(client.GetUserCategories(note._creator));
            _noteCategories = new ObservableCollection<NoteCategoriesModel>(client.GetNoteCategories(note._id));

            foreach (var k in _userCategories)
            {
                if (_noteCategories.Any(x => x._id == k._id)) k.IsFromCategory = true;
                else k.IsFromCategory = false;

                k._noteId = note._id;
            }
        }
        public ObservableCollection<NoteCategoriesModel> UserCategories
        {
            get => _userCategories;
            set
            {
                _userCategories = value;
                NotifyPropertyChanged("UserCategories");
            }
        }
    }
}