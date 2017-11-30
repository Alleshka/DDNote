using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigDesNote.Model;
using System.ComponentModel;

namespace DigDesNote.UI.WPF2Binding.Model
{
    public class NoteModel : Note, INotifyPropertyChanged, ICloneable
    {
        public override string _title
        {
            get => base._title;
            set
            {
                base._title = value;
                NotifyPropertyChanged("_title");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            return new NoteModel()
            {
                _title = this._title,
                _content = this._content,
                _id = this._id,
                _creator = this._creator,
                _created = this._created,
                _updated = this._updated
            };
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
