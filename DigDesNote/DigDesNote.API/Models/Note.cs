using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DigDesNote.Model;

namespace DigDesNote.API.Models
{
    public class BasicNote
    {
        public Guid _id { get; set; } // ID заметки 

        public String _title { get; set; } // Заголовок заметки
        public String _content { get; set; } // Содержимое заметки 
        public Guid _creator { get; set; } // Создатель заметки 

        public DateTime _created { get; set; } // Дата создания 
        public DateTime _updated { get; set; } // Дата редактирования 

        public BasicNote(Note note)
        {
            _id = note._id;
            _title = note._title;
            _content = note._content;
            _creator = note._creator;
            _created = note._created;
            _updated = note._updated;
        }
    }

    public class UpdateNote
    {
        [Required(ErrorMessage = "Укажите ID заметки (_id)")]
        public Guid _id { get; set; } // ID заметки 
        [Required(ErrorMessage ="Необходимо указать заголовок (_title)")]
        public String _title { get; set; } // Заголовок заметки
        [Required(ErrorMessage = "Необходимо указать содержимое (_content)")]
        public String _content { get; set; } // Содержимое заметки 
    }

    public class CreateNote
    {
        [Required(ErrorMessage = "Укажите ID создателя (_creator)")]
        public Guid _creator { get; set; }
        [Required(ErrorMessage = "Необходимо указать заголовок (_title)")]
        public String _title { get; set; } // Заголовок заметки
        [Required(ErrorMessage = "Необходимо указать содержимое (_content)")]
        public String _content { get; set; } // Содержимое заметки 
    }
}