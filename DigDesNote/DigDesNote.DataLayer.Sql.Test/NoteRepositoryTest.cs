using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigDesNote.Model;
using System.Collections.Generic;
using System.Linq;

namespace DigDesNote.DataLayer.Sql.Test
{
    [TestClass]
    public class NoteRepositoryTest
    {
        private List<User> _tmpUs;
        public NoteRepositoryTest()
        {
            _tmpUs = new List<User>();
        }

        [TestMethod]
        /// Проверка добавления замеки и чтения информации из заметок пользователя
        public void CreateNodeReadNotes()
        {
            Note testNote = CommonTestClass.GetRandNote();
            User testUser = CommonTestClass.GetRandUser();

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(CommonTestClass.ConnectionString);
            NoteRepository noteRepository = new NoteRepository(CommonTestClass.ConnectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, noteRepository, categoryRepository);

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
            Note testNote = CommonTestClass.GetRandNote();
            User testUser = CommonTestClass.GetRandUser();

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(CommonTestClass.ConnectionString);
            NoteRepository noteRepository = new NoteRepository(CommonTestClass.ConnectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, noteRepository, categoryRepository);

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
            User testUs = CommonTestClass.GetRandUser();
            Note testNote = CommonTestClass.GetRandNote();
            Category testCategory = CommonTestClass.GetRandCategory();
            Category testCategory2 = CommonTestClass.GetRandCategory(); testCategory2._name = testCategory._name + "2";

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(CommonTestClass.ConnectionString);
            NoteRepository noteRepository = new NoteRepository(CommonTestClass.ConnectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, noteRepository, categoryRepository);

            testUs = userRepository.Create(testUs._login, testUs._email, testUs._pass);
            testNote = noteRepository.Create(testNote._title, testNote._content, testUs._id);

            testCategory = categoryRepository.Create(testUs._id, testCategory._name);
            testCategory2 = categoryRepository.Create(testUs._id, testCategory2._name);

            _tmpUs.Add(testUs);

            noteRepository.AddToCategory(testNote._id, testCategory._id); // Добавляем в категорию
            noteRepository.AddToCategory(testNote._id, testCategory2._id); // Добавляем во вторую категорию

            // asserts
            Assert.AreEqual(categoryRepository.GetNoteCategories(testNote._id).OrderBy(x => x._name).First()._name, testCategory._name);
            Assert.AreEqual(categoryRepository.GetNoteCategories(testNote._id).OrderBy(x => x._name).Last()._name, testCategory2._name);
        }

        [TestMethod]
        /// Удаление заметки из категории
        public void RemoveFromCat()
        {
            // arrange 
            Category testCategory = CommonTestClass.GetRandCategory();
            Note testNote = CommonTestClass.GetRandNote();
            User testUser = CommonTestClass.GetRandUser();

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(CommonTestClass.ConnectionString);
            NoteRepository noteRepository = new NoteRepository(CommonTestClass.ConnectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, noteRepository, categoryRepository);

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            testCategory = categoryRepository.Create(testUser._id, testCategory._name);
            testNote = noteRepository.Create(testNote._title, testNote._content, testUser._id);

            _tmpUs.Add(testUser);

            noteRepository.AddToCategory(testNote._id, testCategory._id); // Добавляем в категорию 
            int i = categoryRepository.GetNoteCategories(testNote._id).Count();
            noteRepository.RemoveFromCategory(testNote._id, testCategory._id); // Удаляем 
            int j = categoryRepository.GetNoteCategories(testCategory._id).Count();

            // Assepts 
            Assert.AreEqual(i, j + 1);
        }

        [TestMethod]
        /// Проверка редактирования заметки
        public void EditNote()
        {
            // arrange 
            Note testNote = CommonTestClass.GetRandNote();
            User testUser = CommonTestClass.GetRandUser();

            // act 
            CategoryRepository cat = new CategoryRepository(CommonTestClass.ConnectionString);
            NoteRepository noteRepository = new NoteRepository(CommonTestClass.ConnectionString, cat);
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, noteRepository, cat);

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
        // Проверка чтения всех заметок пользователя (Личных и расшареных) + проверка на Unshare
        public void GetAllNotes()
        {
            User testUser1 = CommonTestClass.GetRandUser(); // Первый юзер 
            User testUser2 = CommonTestClass.GetRandUser(); // Второй юзер

            Note testNote1 = CommonTestClass.GetRandNote();
            Note testNote2 = CommonTestClass.GetRandNote();
            Note testNote3 = CommonTestClass.GetRandNote();
            Note testNote4 = CommonTestClass.GetRandNote();

            // act 
            String _connect = CommonTestClass.ConnectionString;
            NoteRepository noteRepository = new NoteRepository(_connect, new CategoryRepository(_connect));
            UserRepository userRepository = new UserRepository(_connect, noteRepository, new CategoryRepository(_connect));

            // Создаём пользователей
            testUser1 = userRepository.Create(testUser1._login, testUser1._email, testUser1._pass);
            testUser2 = userRepository.Create(testUser2._login, testUser2._email, testUser2._pass);

            _tmpUs.Add(testUser1);
            _tmpUs.Add(testUser2);

            // Создаём заметки 
            testNote1 = noteRepository.Create(testNote1._title, testNote1._content, testUser1._id);
            testNote2 = noteRepository.Create(testNote2._title, testNote2._content, testUser1._id);
            testNote3 = noteRepository.Create(testNote3._title, testNote3._content, testUser2._id);
            testNote4 = noteRepository.Create(testNote4._title, testNote4._content, testUser2._id);

            // Шарим 2 заметки 
            noteRepository.Share(testNote3._id, testUser1._id);
            noteRepository.Share(testNote4._id, testUser1._id);

            int createUs1 = noteRepository.GetUserNotes(testUser1._id).Count();
            int shareUs1 = noteRepository.GetShareUserNotes(testUser1._id).Count();
            int totalUs1 = createUs1 + shareUs1;
            int totalAllUs1 = noteRepository.GetAllUserNotes(testUser1._id).Count();

            int createUs2 = noteRepository.GetUserNotes(testUser2._id).Count();
            int shareUs2 = noteRepository.GetShareUserNotes(testUser2._id).Count();
            int totalUs2 = createUs2 + shareUs2;


            noteRepository.UnShare(testNote3._id, testUser1._id); // Убираем
            int newUsshar = noteRepository.GetUserNotes(testUser1._id).Count() + noteRepository.GetShareUserNotes(testUser1._id).Count();

            // Asserts 
            Assert.AreEqual(createUs1, 2);
            Assert.AreEqual(shareUs1, 2);
            Assert.AreEqual(totalUs1, 4);
            Assert.AreEqual(createUs2, 2);
            Assert.AreEqual(shareUs2, 0);
            Assert.AreEqual(totalUs2, 2);
            Assert.AreEqual(newUsshar, 3);
            Assert.AreEqual(totalAllUs1, totalUs1);
        }

        [TestMethod]
        public void DeleteNote()
        {
            Note testNote1 = CommonTestClass.GetRandNote();
            Note testNote2 = CommonTestClass.GetRandNote();

            User testUser = CommonTestClass.GetRandUser();

            // act 
            String _connectionString = CommonTestClass.ConnectionString;
            NoteRepository noteRepository = new NoteRepository(_connectionString, new CategoryRepository(_connectionString));
            UserRepository userRepository = new UserRepository(_connectionString, noteRepository, new CategoryRepository(_connectionString));

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            _tmpUs.Add(testUser);

            testNote1 = noteRepository.Create(testNote1._title, testNote1._content, testUser._id);
            testNote2 = noteRepository.Create(testNote2._title, testNote2._content, testUser._id);

            int i = noteRepository.GetUserNotes(testUser._id).Count();
            noteRepository.Delete(testNote1._id);
            int j = noteRepository.GetUserNotes(testUser._id).Count();

            // Asserts 
            Assert.AreEqual(i, j + 1);
            Assert.AreEqual(j, 1);
        }

        [TestCleanup]
        public void CleanData()
        {
            foreach (var temp in _tmpUs)
                new UserRepository(CommonTestClass.ConnectionString, new NoteRepository(CommonTestClass.ConnectionString, new CategoryRepository(CommonTestClass.ConnectionString)), new CategoryRepository(CommonTestClass.ConnectionString)).Delete(temp._id);
        }
    }
}