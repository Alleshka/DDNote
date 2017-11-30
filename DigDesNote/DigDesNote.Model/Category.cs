using System;
using System.ComponentModel.DataAnnotations;

namespace DigDesNote.Model
{
    public class Category
    {
        public Guid _id { get; set; }  // ID категории
        [Required(ErrorMessage = "Укажите название категории")]
        [StringLength(45, MinimumLength = 1, ErrorMessage = "Название категории должно содержать от 1 до 45 символов")]
        public virtual String _name { get; set; } // Название категории 
        public Guid _userId { get; set; }
    }
}