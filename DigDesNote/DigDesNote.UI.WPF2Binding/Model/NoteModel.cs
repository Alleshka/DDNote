using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigDesNote.Model;
using System.ComponentModel;

namespace DigDesNote.UI.WPF2Binding.Model
{
    public class NoteModel : Note, INotifyPropertyChanged
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
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
