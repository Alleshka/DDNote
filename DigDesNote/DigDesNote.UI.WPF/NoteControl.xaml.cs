using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
