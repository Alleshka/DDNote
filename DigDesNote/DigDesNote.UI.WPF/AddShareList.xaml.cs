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
    /// Логика взаимодействия для AddShareList.xaml
    /// </summary>
    public partial class AddShareList : Window
    {
        private List<User> _userList;

        private ServiceClient _client;
        private Guid _noteId;

        bool redact;

        public AddShareList(ServiceClient client, Guid noteId)
        {
            InitializeComponent();

            _client = client;
            _noteId = noteId;

            RefreshList();

            redact = false;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _client.ShareNoteToUser(_login.Text, _noteId);
            RefreshList();
        }

        private void RefreshList()
        {
            _shareList.ItemsSource = null;
            _userList = (from us in _client.GetShares(_noteId) select _client.GetBasicUserInfo(us)).OrderBy(x => x._id).ToList(); // Получаем список юзеров 
            _shareList.ItemsSource = (from us in _userList select us._login);

            redact = true;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _client.UnShareNoteToUser(_userList[_shareList.SelectedIndex]._id, _noteId); // Убираем шару
            RefreshList();
        }

        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = redact;
        }
    }
}
