using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigDesNote.Model;
using System.ComponentModel;
using DigDesNote.UI.WPF2Binding.Code;

namespace DigDesNote.UI.WPF2Binding.Model
{
    public class NoteCategoriesModel : Category, INotifyPropertyChanged
    {
        private bool _noteFromCategory;

        public Guid _noteId = new Guid();

        public override string _name
        {
            get => base._name;
            set
            {
                base._name = value;
                NotifyPropertyChanged("_name");
            }
        }

        public bool IsFromCategory
        {
            get => _noteFromCategory;
            set
            {
                _noteFromCategory = value;

                if (_noteId != Guid.Empty)
                {
                    if (_noteFromCategory) new ServiceClient("http://localhost:41606/api/").AddNoteToCategory(_noteId, _id);
                    else new ServiceClient("http://localhost:41606/api/").DelNoteToCategory(_noteId, _id);
                }

                NotifyPropertyChanged("IsFromCategory");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}