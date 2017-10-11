using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigDesNote.Model;
using DigDesNote.DataLayer.Sql;
using System.Collections.Generic;
using System.Linq;

namespace DigDesNote.DataLayer.Sql.Test
{
    [TestClass]
    public class ReposioryTests
    {
        private const String _connectionString = @"Data Source=DESKTOP-H4JQP0V;Initial Catalog=NoteDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private List<User> _tmpUs;

        public ReposioryTests()
        {
            _tmpUs = new List<User>();
        }

        private User GetTestUser()
        {
            return new User()
            {
                _login = "TestLogin",
                _email = "testedafaf@mail.ru",
                _pass = "Password1234",
            };
        }
        private Note GetTestNote()
        {
            return new Note()
            {
                _title = "TestTitle",
                _content = "This is a very very very very very very bad code :("
            };
        }
        private Category GetTestCategory()
        {
            return new Category()
            {
                _name = "TestCagegory"
            };
        }

        [TestMethod]
        ///Проверка создания пользователя и чтения инфы из БД
        public void ShouldCreateUser()
        {
            // arrange 
            User testUser = GetTestUser();

            // act
            CategoryRepository categoryRepository = new CategoryRepository(_connectionString);
            NoteRepository noteRepository = new NoteRepository(_connectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(_connectionString, noteRepository, categoryRepository);
            User usDb = userRepository.Create(testUser._login, testUser._email, testUser._pass); // Добавляем пользователя
            _tmpUs.Add(usDb);

            testUser = userRepository.Get(usDb._id);

            // Asserts
            Assert.AreEqual(usDb._login, testUser._login);
        }

        [TestMethod]
        /// Проверка добавления замтки и чтения информации из заметок пользователя
        public void CreateNodeReadNotes()
        {
            Note testNote = GetTestNote();
            User testUser = GetTestUser(); 

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(_connectionString);
            NoteRepository noteRepository = new NoteRepository(_connectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(_connectionString, noteRepository, categoryRepository);

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            testNote = noteRepository.Create(testNote._title, testNote._content, testUser._id);

            _tmpUs.Add(testUser);

            // Asserts
            Assert.AreEqual(testNote._id, noteRepository.GetUserNotes(testUser._id).Single()._id);
        }

        [TestMethod]
        /// Проверка добавления заметки и чтения информации по ID
        public void CreateNoteReadeOneNote()
        {
            Note testNote = GetTestNote();
            User testUser = GetTestUser();

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(_connectionString);
            NoteRepository noteRepository = new NoteRepository(_connectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(_connectionString, noteRepository, categoryRepository);

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            testNote = noteRepository.Create(testNote._title, testNote._content, testUser._id);

            Note noteDb = noteRepository.GetNote(testNote._id);

            _tmpUs.Add(testUser);

            // Asserts
            Assert.AreEqual(testNote._title, noteDb._title);
        }

        [TestMethod]
        /// Проверка добавления заметки в категории
        public void AddNoteToCat()
        {
            // arrange 
            User testUs = GetTestUser();
            Note testNote = GetTestNote();
            Category testCategory = GetTestCategory();
            Category testCategory2 = GetTestCategory(); testCategory2._name += "2";

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(_connectionString);
            NoteRepository noteRepository = new NoteRepository(_connectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(_connectionString, noteRepository, categoryRepository);

            testUs = userRepository.Create(testUs._login, testUs._email, testUs._pass);
            testNote = noteRepository.Create(testNote._title, testNote._content, testUs._id);

            testCategory = categoryRepository.Create(testUs._id, testCategory._name);
            testCategory2 = categoryRepository.Create(testUs._id, testCategory2._name);

            _tmpUs.Add(testUs);

            noteRepository.AddToCategory(testNote._id, testCategory._id); // Добавляем в категорию
            noteRepository.AddToCategory(testNote._id, testCategory2._id); // Добавляем во вторую категорию

            // asserts
            Assert.AreEqual(categoryRepository.GetNoteCategories(testNote._id).OrderBy(x=>x._name).First()._name, testCategory._name);
            Assert.AreEqual(categoryRepository.GetNoteCategories(testNote._id).OrderBy(x => x._name).Last()._name, testCategory2._name);
        }

        [TestMethod]
        /// Проверка Внесения изменений в категорию 
        public void UpdateCat()
        {
            // arrange 
            Category testCategory = GetTestCategory();
            User testUser = GetTestUser();

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(_connectionString);
            UserRepository userRepository = new UserRepository(_connectionString, new NoteRepository(_connectionString, categoryRepository), categoryRepository);

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass); 
            testCategory = categoryRepository.Create(testUser._id, testCategory._name);

            _tmpUs.Add(testUser);

            Category newCat = categoryRepository.Edit(testCategory._id, testCategory._name + "666");

            // Asserts 
            Assert.AreEqual(testCategory._name, GetTestCategory()._name);
            Assert.AreEqual(testCategory._id, newCat._id);
            Assert.AreNotEqual(newCat._name, GetTestCategory()._name);
        }

        [TestMethod]
        /// Удаление заметки из категории
        public void RemoveFromCat()
        {
            // arrange 
            Category testCategory = GetTestCategory();
            Note testNote = GetTestNote();
            User testUser = GetTestUser();

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(_connectionString);
            NoteRepository noteRepository = new NoteRepository(_connectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(_connectionString, noteRepository, categoryRepository);

            testUser =  userRepository.Create(testUser._login, testUser._email, testUser._pass);
            testCategory = categoryRepository.Create(testUser._id, testCategory._name);
            testNote = noteRepository.Create(testNote._title, testNote._content, testUser._id);

            _tmpUs.Add(testUser);

            noteRepository.AddToCategory(testNote._id, testCategory._id); // Добавляем в категорию 
            int i = categoryRepository.GetNoteCategories(testNote._id).Count(); 
            noteRepository.RemoveFromCagegory(testNote._id, testCategory._id); // Удаляем 
            int j = categoryRepository.GetNoteCategories(testCategory._id).Count();

            // Assepts 
            Assert.AreEqual(i, j + 1);
        }

        [TestMethod]
        /// Проверка редактирования заметки
        public void EditNote()
        {
            // arrange 
            Note testNote = GetTestNote();
            User testUser = GetTestUser();

            // act 
            CategoryRepository cat = new CategoryRepository(_connectionString);
            NoteRepository noteRepository = new NoteRepository(_connectionString, cat);
            UserRepository userRepository = new UserRepository(_connectionString, noteRepository, cat);

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            _tmpUs.Add(testUser);
            testNote = noteRepository.Create(testNote._title, testNote._content, testUser._id); // Создали заметку 
            noteRepository.Edit(testNote._id, "Я новый заголовок", "А я новое содержимое");
            Note newNote = noteRepository.GetNote(testNote._id);

            // Asserts
            Assert.AreEqual(testNote._id, newNote._id);
            Assert.AreNotEqual(testNote._updated, newNote._updated);
            Assert.AreEqual(newNote._content, "А я новое содержимое");

        }

        [TestMethod]
        /// Внесение изменений в пользователя
        public void EditUser()
        {
            User testUser = GetTestUser();

            // act 
            UserRepository userRepository = new UserRepository(_connectionString, new NoteRepository(_connectionString, new CategoryRepository(_connectionString)), new CategoryRepository(_connectionString));
            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            _tmpUs.Add(testUser);

            User newUser = userRepository.Edit(testUser._id, "newMail@mail.ru", "newPass");

            // asserts
            Assert.AreEqual(testUser._id, newUser._id);
            Assert.AreNotEqual(testUser._email, newUser._email);
        }

        [TestCleanup]
        public void CleanData()
        {
            foreach (var temp in _tmpUs)
                new UserRepository(_connectionString, new NoteRepository(_connectionString, new CategoryRepository(_connectionString)), new CategoryRepository(_connectionString)).Delete(temp._id);
        }
    }
}
