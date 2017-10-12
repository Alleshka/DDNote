using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DigDesNote.Model;

namespace DigDesNote.DataLayer.Sql.Test
{
    public static class CommonTestClass
    {
        public static User GetRandUser()
        {
            Guid f = Guid.NewGuid();
            return new User()
            {
                _login = f.ToString(),
                _email = f.ToString() + "@gr.ru",
                _pass = f.ToString()
            };
        }
        //public static User GetTestUser()
        //{
        //    return new User()
        //    {
        //        _login = "TestLogin",
        //        _email = "testedafaf@mail.ru",
        //        _pass = "Password1234",
        //    };
        //}

        public static Note GetRandNote()
        {
            Guid f = Guid.NewGuid();
            return new Note()
            {
                _title = f.ToString(),
                _content = f.ToString() + f.ToString()
            };
        }

        //public static Note GetTestNote()
        //{
        //    return new Note()
        //    {
        //        _title = "TestTitle",
        //        _content = "This is a very very very very very very bad code :("
        //    };
        //}
        public static Category GetRandCategory()
        {
            Guid f = Guid.NewGuid();
            return new Category()
            {
                _name = f.ToString()
            };
        }
        //public static Category GetTestCategory()
        //{
        //    return new Category()
        //    {
        //        _name = "TestCagegory"
        //    };
        //}
        public static String ConnectionString => @"Data Source=DESKTOP-H4JQP0V;Initial Catalog=NoteDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}