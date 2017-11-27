using System;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    public class UserViewModel : BaseViewModel
    {
        public UserViewModel(User user)
        {
            _curUser = user;
        }

        private User _curUser;

        public String Login
        {
            get => _curUser._login;
        }
        public Guid UserId
        {
            get => _curUser._id;
        }
    }
}
