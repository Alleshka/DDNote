using System.Windows.Input;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;
using System.Configuration;
using System.Collections.ObjectModel;
using DigDesNote.Model;
using System;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class SharesListViewModel : BaseViewModel
    {
        ServiceClient _client;
        NoteModel _note;

        // Список пользователей, которым доступно
        private ObservableCollection<User> _userList;
        public ObservableCollection<User> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                NotifyPropertyChanged("UserList");
            }
        }

        // Логин создателя
        private String _login;
        public String Login
        {
            get => _login;
            set
            {
                _login = value;
                NotifyPropertyChanged("Login");
            }
        }

        /// <summary>
        /// Выбранный пользователь
        /// </summary>
        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                NotifyPropertyChanged("SelectedUser");
            }
        }

        public SharesListViewModel(NoteModel note)
        {
            _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
            _note = note;
            _userList = new ObservableCollection<User>(_client.GetNoteShares(_note._id));
            Login = "";
        }

        /// <summary>
        /// Добавить юзера в шары
        /// </summary>
        public ICommand AddUserCommand
        {
            get => new BaseCommand((object par) =>
            {
                try
                {
                    if (Login.Length == 0) throw new Exception("Необходимо ввести логин");

                    _client.ShareNoteToUser(Login, _note._id);
                    _userList.Add(_client.GetBasicUserInfo(Login));
                    Login = "";
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }

        /// <summary>
        /// Удалить пользователя из шар
        /// </summary>
        public ICommand DelUserCommand
        {
            get => new BaseCommand((object par)=>
            {
                try
                {
                    if (UserList.Count == 0) throw new Exception("Пользователи отсутствуют");
                    if (SelectedUser == null) throw new Exception("Необходимо выбрать пользователя");
                    _client.UnShareNoteToUser(SelectedUser._id, _note._id);
                    _userList.Remove(SelectedUser);
                    Login = "";
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }
    }
}