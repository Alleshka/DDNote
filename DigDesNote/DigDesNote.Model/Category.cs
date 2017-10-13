using System;

namespace DigDesNote.Model
{
    public class Category
    {
        public Guid _id { get; set; }  // ID категории

        public String _name { get; set; } // Название категории 

        public Guid _userId { get; set; }
    }
}