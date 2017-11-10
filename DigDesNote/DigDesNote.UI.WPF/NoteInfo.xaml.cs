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
    /// Логика взаимодействия для NoteInfo.xaml
    /// </summary>
    public partial class NoteInfo : Window
    {
        private Guid _noteId;
        private ServiceClient _client;
        private Note _note;

        private bool reduct = false;

        public NoteInfo(ServiceClient clietn, Guid note)
        {
            InitializeComponent();
            _noteId = note;
            _client = clietn;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _note = _client.GetBasicNoteInfo(_noteId);

            _updateTime.Text = _note._updated.ToString();
            _createTime.Text = _note._created.ToString();

            _noteConrol.Title = _note._title;
            _noteConrol.NoteContent = _note._content;
        }

        // Срабатывает при нажатии "Сохранить"
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _note._content = _noteConrol.NoteContent;
            _note._title = _noteConrol.Title;
            _note = _client.UpdateNote(_note);
            this.DialogResult = true;
        }

        // Срабатывает при нажатии закрыть
        private void BtnClode_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = reduct;
        }

        // Двойной клик по категориям
        private void _categoryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddCategoryWindow adc = new AddCategoryWindow(_client, _note._creator, _noteId);
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
    }
}
