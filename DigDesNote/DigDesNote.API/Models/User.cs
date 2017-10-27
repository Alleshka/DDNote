using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigDesNote.Model;
using System.ComponentModel.DataAnnotations;

namespace DigDesNote.API.Models
{
    public class BasicUser
    {
        public Guid _id { get; set; } // Идентификатор пользователя 
        public String _login { get; set; } // Логин пользователя 
        public String _email { get; set; } // Почта пользователя 

        public BasicUser(User user)
        {
            _id = user._id;
            _login = user._login;
            _email = user._email;
        }
    }
    public class FullUser : BasicUser
    {
        public FullUser(User user) : base(user)
        {
            _categories = user._categories;
            _notes = user._notes;
        }

        public IEnumerable<Category> _categories { get; set; } // Категории пользователя 
        public IEnumerable<Note> _notes { get; set; } // Заметки пользователя 
    }

    public class CreateUser
    {
        [Required(ErrorMessage ="Укажите логин пользователя (_login)")]
        public String _login { get; set; } // Логин пользователя 
        [Required(ErrorMessage = "Укажите e-mail пользователя (_email)")]
        public String _email { get; set; } // Почта пользователя 
        [Required(ErrorMessage = "Укажите пароль пользователя (_password)")]
        public String _password { get; set; }
    }

    public class EditUser
    {
        [Required(ErrorMessage ="Укажите ID пользователя")]
        public Guid _id { get; set; }
        [Required(ErrorMessage = "Укажите e-mail пользователя (_email)")]
        public String _email { get; set; } // Почта пользователя 
        [Required(ErrorMessage = "Укажите пароль пользователя (_password)")]
        public String _password { get; set; }
    }
}