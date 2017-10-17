using System;
using System.Collections.Generic;

namespace DigDesNote.Model
{
    public class Note
    {
        public Guid _id { get; set; } // ID заметки 
        public String _title { get; set; } // Заголовок заметки
        public String _content { get; set; } // Содержимое заметки 
        public Guid _creator { get; set; } // Создатель заметки 

        public DateTime _created { get; set; } // Дата создания 
        public DateTime _updated { get; set; } // Дата редактирования 

        public IEnumerable<Category> _categories { get; set; } // Список категорий 
        public IEnumerable<Guid> _shares { get; set; } // Список расшаренных пользователей
    }
}