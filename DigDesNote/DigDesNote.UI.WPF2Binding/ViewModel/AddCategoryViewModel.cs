using System.Collections.ObjectModel;
using System.Linq;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Command;
using System.Configuration;
using System.Windows.Input;
using DigDesNote.Model;
using System;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class AddCategoryViewModel : BaseViewModel
    {
        // Текущая заметка
        NoteModel _curNote;

        // Список категорий пользователя
        ObservableCollection<NoteCategoriesModel> _userCategories;

        // Список заметок в категории
        ObservableCollection<NoteCategoriesModel> _noteCategories;

        ServiceClient _client;

        public AddCategoryViewModel(NoteModel note)
        {
            _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
            _curNote = note;
            
            _userCategories = new ObservableCollection<NoteCategoriesModel>(_client.GetUserCategories(note._creator));
            _noteCategories = new ObservableCollection<NoteCategoriesModel>(_client.GetNoteCategories(note._id));

            
            foreach (var k in _userCategories)
            {
                // Проставляем лежит ли текущая заметка в категории
                if (_noteCategories.Any(x => x._id == k._id)) k.IsFromCategory = true;
                else k.IsFromCategory = false;

                // Ставим id заметки
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