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
using System.Windows.Shapes;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для CreateNoteWindow.xaml
    /// </summary>
    public partial class CreateNoteWindow : Window
    {
        private readonly ServiceClient _client;
        private readonly Guid _curId;

        public CreateNoteWindow(ServiceClient client, Guid id)
        {
            InitializeComponent();
            _client = client;
            _curId = id;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Note newNote = new Note()
                {
                    _title = _noteControl.Title,
                    _content = _noteControl.NoteContent,
                    _creator = _curId
                };

                newNote = _client.CreateNote(newNote);
                // _client.SynchronizationNotes(_curId);
                MessageBox.Show("Заметка успешно создана");

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
