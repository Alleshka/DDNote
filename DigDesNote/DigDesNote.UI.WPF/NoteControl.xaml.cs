using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для NoteControl.xaml
    /// </summary>
    public partial class NoteControl : UserControl
    {
        public NoteControl()
        {
            InitializeComponent();
        }

        public String Title
        {
            get => _title.Text;
            set => _title.Text = value;
        }
        public String NoteContent
        {
            get => _content.Text;
            set => _content.Text = value;
        }

        private void _content_KeyUp(object sender, KeyEventArgs e)
        {
        }
    }
}
