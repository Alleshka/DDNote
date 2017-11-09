using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace DigDesNote.Model
{
    public class User
    {
        public Guid _id { get; set; } // Идентификатор пользователя 

        [Required(ErrorMessage = "Укажите логин (_login)")]

        public String _login { get; set; } // Логин пользователя 

        [Required(ErrorMessage = "Укажите почту (_email)")]
        [RegularExpression(@"\w+" + @"@\w+" + @"\.\w+", ErrorMessage = "Укажите корректный адрес")]

        public String _email { get; set; } // Почта пользователя 
        [Required(ErrorMessage = "Укажите пароль (_pass)")]

        public String _pass { get; set; } // Пароль пользователя

        public IEnumerable<Category> _categories { get; set; } // Категории пользователя 
        public IEnumerable<Note> _notes { get; set; } // Заметки пользователя 
    }
}