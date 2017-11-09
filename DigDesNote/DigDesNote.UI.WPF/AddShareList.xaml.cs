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

        public AddShareList(ServiceClient client, Guid noteId)
        {
            InitializeComponent();

            _client = client;
            _noteId = noteId;

            RefreshList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _client.ShareNote(_login.Text, _noteId);
            RefreshList();
        }

        private void RefreshList()
        {
            _userList = (from us in _client.GetShares(_noteId) select _client.GetBasicUserInfo(us)).ToList();
            _shareList.ItemsSource = from k in _userList select k._login;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _client.UnShareNote(_userList[_shareList.SelectedIndex]._id, _noteId);
            RefreshList();
        }

        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
