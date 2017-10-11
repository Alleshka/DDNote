using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DigDesNote.Model
{
    public class Category
    {
        public Guid _id { get; set; }  // ID категории

        public String _name { get; set; } // Название категории 

        public Guid _userId { get; set; }
    }
}