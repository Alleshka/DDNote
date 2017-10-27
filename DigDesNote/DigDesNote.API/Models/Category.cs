using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DigDesNote.API.Models
{
    /// <summary>
    /// В помощь для переименовывания категории
    /// </summary>
    public class CategoryRename
    {
        [Required(ErrorMessage = "Требуется указать ID категории (_id)")]
        public Guid _id { get; set; }
        [Required(ErrorMessage = "Требуется указать название категории (_name)")]
        public String _name { get; set; }
    }

    /// <summary>
    /// В помощь для создания категорий
    /// </summary>
    public class CategoryCreate
    {
        [Required(ErrorMessage ="Требуется указать ID пользователя (_userId)")]
        public Guid _userId { get; set; }

        [Required(ErrorMessage = "Требуется указать название категории (_name)")]
        public String _name { get; set; } 
    }
}