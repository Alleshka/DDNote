using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigDesNote.Model
{
    public class User
    {
        public Guid _id { get; set; } // Идентификатор пользователя 
        public String _login { get; set; } // Логин пользователя 
        public String _email { get; set; } // Почта пользователя 
        public String _pass { get; set; } // Пароль пользователя

        public IEnumerable<Category> _categories { get; set; } // Категории пользователя 
        public IEnumerable<Note> _notes { get; set; } // Заметки пользователя 
    }
}