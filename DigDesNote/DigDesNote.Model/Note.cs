using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigDesNote.Model
{
    public class Note
    {
        public virtual Guid _id { get; set; } // ID заметки 

        [Required(ErrorMessage = "Укажите название заметки ")]
        [StringLength(45, MinimumLength = 1, ErrorMessage ="Название заметки должно быть от 1 до 45 символов")]
        public virtual String _title { get; set; } // Заголовок заметки

        [Required(ErrorMessage = "Укажите содержимое заметки")]
        public virtual String _content { get; set; } // Содержимое заметки 

        public virtual Guid _creator { get; set; } // Создатель заметки 

        public virtual DateTime _created { get; set; } // Дата создания 
        public virtual DateTime _updated { get; set; } // Дата редактирования 

        public virtual IEnumerable<Category> _categories { get; set; } // Список категорий 
        public virtual IEnumerable<Guid> _shares { get; set; } // Список расшаренных пользователей
    }
}