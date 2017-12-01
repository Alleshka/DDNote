using System;
using DigDesNote.Model;
using System.ComponentModel;

namespace DigDesNote.UI.WPF2Binding.Model
{
    public class NoteModel : Note, INotifyPropertyChanged, ICloneable
    {
        private String _login;

        public String LoginAutor
        {
            get => _login;
            set
            {
                _login = value;
                NotifyPropertyChanged("LoginAutor");
            }
        }

        public String ViewTitle
        {
            get => _title + $" ({LoginAutor})";
        }
        public override string _title
        {
            get => base._title;
            set
            {
                base._title = value;            
                NotifyPropertyChanged("_title");
                NotifyPropertyChanged("ViewTitle");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            return new NoteModel()
            {
                _title = this._title,
                _content = this._content,
                _id = this._id,
                _creator = this._creator,
                _created = this._created,
                _updated = this._updated,
                _login = this.LoginAutor
            };
        }
    }
}
