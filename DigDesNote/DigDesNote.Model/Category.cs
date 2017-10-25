using System;
using System.ComponentModel.DataAnnotations;

namespace DigDesNote.Model
{
    public class Category
    {
        public Guid _id { get; set; }  // ID категории

        [Required]
        public String _name { get; set; } // Название категории 

        [Required]
        public Guid _userId { get; set; }
    }
}