using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для NoteInfo.xaml
    /// </summary>
    public partial class NoteInfo : Window
    {
        private Guid _noteId;
        private ServiceClient _client;
        private Note _note;
        private Guid _userId;

        private bool reduct = false;

        public NoteInfo(ServiceClient clietn, Guid note, Guid userId)
        {
            InitializeComponent();

            _noteId = note;
            _client = clietn;
            _userId = userId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _note = _client.GetBasicNoteInfo(_noteId);

            _updateTime.Text = _note._updated.ToString();
            _createTime.Text = _note._created.ToString();

            _noteConrol.Title = _note._title;
            _noteConrol.NoteContent = _note._content;

            if (_userId != _note._creator)
            {
                ShareExpand.IsEnabled = false;
                _categoryExpand.IsEnabled = false;
            }
        }

        // Срабатывает при нажатии "Сохранить"
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _note._content = _noteConrol.NoteContent;
                _note._title = _noteConrol.Title;
                _note = _client.UpdateNote(_note);
                reduct = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Срабатывает при нажатии закрыть
        private void BtnClode_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = reduct;
        }

        // Двойной клик по категориям
        private void _categoryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddCategoryW adc = new AddCategoryW(_client, _note._creator, _noteId);
            if (adc.ShowDialog() == true)
            {
                _categoryList.ItemsSource = from cat in _client.GetNoteCategories(_noteId) select cat._name;
                reduct = true;
            }
        }

        // Двойной клик по шарам
        private void _shareList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddShareList asl = new AddShareList(_client, _noteId);
            if (asl.ShowDialog() == true)
            {
                // _client.SynchronizationNotes(_client.GetBasicNoteInfo(_noteId)._creator);
                _shareList.ItemsSource = from shar in _client.GetShares(_noteId) select _client.GetBasicUserInfo(shar)._login;
                reduct = true;
            }
        }

        // Развернуть список пользователей, которым доступно
        private void ShareExpand_Expanded(object sender, RoutedEventArgs e)
        {
            if (_shareList.ItemsSource == null)
            {
                _shareList.ItemsSource = _shareList.ItemsSource = from share in _client.GetShares(_noteId) select _client.GetBasicUserInfo(share)._login;
            }
        }

        // Развернуть список категорий
        private void _categoryExpand_Expanded(object sender, RoutedEventArgs e)
        {
            if (_categoryList.ItemsSource == null)
            {
                _categoryList.ItemsSource = from cat in _client.GetNoteCategories(_noteId) select cat._name;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = reduct;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddShareList asl = new AddShareList(_client, _noteId);
            if (asl.ShowDialog() == true)
            {
                // _client.SynchronizationNotes(_client.GetBasicNoteInfo(_noteId)._creator);
                _shareList.ItemsSource = from shar in _client.GetShares(_noteId) select _client.GetBasicUserInfo(shar)._login;
                reduct = true;
            }
        }
    }
}
