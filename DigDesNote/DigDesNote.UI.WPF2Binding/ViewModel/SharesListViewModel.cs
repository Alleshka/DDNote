using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using DigDesNote.UI.WPF2Binding.Model;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.Model;
using System.Windows.Input;


namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class SharesListViewModel : BaseViewModel
    {
        private ObservableCollection<User> _userList;
        private String _login;

        ServiceClient _client;
        Guid _noteId;

        private User _selectedUser;

        public SharesListViewModel(Guid noteId, ServiceClient client)
        {
            _client = client;
            _noteId = noteId;
            _userList = new ObservableCollection<User>(_client.GetNoteShares(noteId));
        }

        public ObservableCollection<User> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                NotifyPropertyChanged("UserList");
            }
        }

        public String Login
        {
            get => _login;
            set
            {
                _login = value;
                NotifyPropertyChanged("Login");
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                NotifyPropertyChanged("SelectedUser");
            }
        }

        public ICommand AddUserCommand
        {
            get => new BaseCommand(AddUser);
        }

        public void AddUser(object par)
        {
            try
            {
                _client.ShareNoteToUser(Login, _noteId);
                _userList.Add(_client.GetBasicUserInfo(Login));
                Login = "";
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public ICommand DelUserCommand
        {
            get => new BaseCommand(DelUser);
        }

        public void DelUser(object par)
        {
            _client.UnShareNoteToUser(SelectedUser._id, _noteId);
            _userList.Remove(SelectedUser);
            Login = "";
        }
    }
}