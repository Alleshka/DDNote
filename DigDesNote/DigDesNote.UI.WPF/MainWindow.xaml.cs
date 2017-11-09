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
using AGLibrary.Files;
using DigDesNote.Model;
using System.Linq;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceClient _client;
        private Guid _curId;

        private List<Note> PersonalNotes = null; // Личные заметки

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _client = new ServiceClient("http://localhost:41606/api/"); // Инициализация домашнего домена

            AGLibrary.Files.FileWork.ReadDataJson<LoginSet>(out LoginSet loginSet, "adm//loginset.json"); // Считываем информацию о входе
            // Если не залогинились, то выводим форму входа
            if ((loginSet == null) || (loginSet._statusLogin == false))
            {
                LoginWindow login = new LoginWindow(_client);
                login.ShowDialog();
            }

            GetCurLogin(); // Получаем текущий id
        }

        private void BtnCreateNote_Click(object sender, RoutedEventArgs e)
        {
            CreateNoteWindow create = new CreateNoteWindow(_client, _curId);
            if(create.ShowDialog()==true) RefreshDataGrid();
        }

        private void GetCurLogin()
        {
            FileWork.ReadDataJson<LoginSet>(out LoginSet loginSet, "adm//loginset.json");
            _curId = loginSet._userId;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
        }

        private void PersListBox_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void PersListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NoteInfo note = new NoteInfo(_client, PersonalNotes[PersListBox.SelectedIndex]._id);
            if (note.ShowDialog() == true)
            {
                PersonalNotes = _client.RefreshNotes(_curId).ToList();
                RefreshDataGrid();
            }
        }

        private void RefreshDataGrid()
        {
            if (PersonalNotes == null)
            {
                PersonalNotes = _client.RefreshNotes(_curId).ToList();  // Получаем личные заметки
            }

            PersListBox.ItemsSource = from pers in PersonalNotes select pers._title;
        }
    }
}
