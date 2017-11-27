using System;
using System.Collections;
using System.Collections.ObjectModel;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class NoteInfoViewModel : BaseViewModel
    {
        private NoteModel _curNote;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<User> _shares;

        public NoteInfoViewModel(NoteModel curNote, ServiceClient client)
        {
            _curNote = curNote;
            _creator = client.GetBasicUserInfo(curNote._creator);
            _categories = new ObservableCollection<Category>(client.GetNoteCategories(curNote._id));
            _shares = new ObservableCollection<User>(client.GetNoteShares(_curNote._id));
        }
        private User _creator; // Создатель

        public String Title1
        {
            get => _curNote._title;
            set
            {
                _curNote._title = value;
                NotifyPropertyChanged("Title1");
            }
        }

        public String Content
        {
            get => _curNote._content;
            set
            {
                _curNote._content = value;
                NotifyPropertyChanged("Content");
            }
        }

        public User Creator
        {
            get => _creator;
        }

        public DateTime Created
        {
            get => _curNote._created;
        }

        public DateTime Updated
        {
            get => _curNote._updated;
            set
            {
                _curNote._updated = value;
                NotifyPropertyChanged("Updated");
            }
        }
        
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                NotifyPropertyChanged("Categories");
            }
        }
        public ObservableCollection<User> Shares
        {
            get => _shares;
            set
            {
                _shares = value;
                NotifyPropertyChanged("Shares");
            }
        }
    }
}
