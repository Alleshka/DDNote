using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigDesNote.Model;
using System.Collections.Generic;

namespace DigDesNote.DataLayer.Sql.Test
{
    [TestClass]
    public class UserRepositoryTest
    {     
        private List<User> _tmpUs;
        public UserRepositoryTest()
        {
            _tmpUs = new List<User>();
        }

        [TestMethod]
        ///Проверка создания пользователя и чтения инфы из БД
        public void ShouldCreateUser()
        {
            // arrange 
            User testUser = CommonTestClass.GetRandUser();

            // act
            CategoryRepository categoryRepository = new CategoryRepository(CommonTestClass.ConnectionString);
            NoteRepository noteRepository = new NoteRepository(CommonTestClass.ConnectionString, categoryRepository);
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, noteRepository, categoryRepository);
            User usDb = userRepository.Create(testUser._login, testUser._email, testUser._pass); // Добавляем пользователя
            _tmpUs.Add(usDb);

            testUser = userRepository.Get(usDb._id);

            // Asserts
            Assert.AreEqual(usDb._login, testUser._login);
        }

        [TestMethod]
        /// Внесение изменений в пользователя
        public void EditUser()
        {
            User testUser = CommonTestClass.GetRandUser();

            // act 
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, new NoteRepository(CommonTestClass.ConnectionString, new CategoryRepository(CommonTestClass.ConnectionString)), new CategoryRepository(CommonTestClass.ConnectionString));
            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            _tmpUs.Add(testUser);

            User newUser = userRepository.Edit(testUser._id, (Guid.NewGuid().ToString() + "@gr.ru"), "newPass");

            // asserts
            Assert.AreEqual(testUser._id, newUser._id);
            Assert.AreNotEqual(testUser._email, newUser._email);
        }

        [TestCleanup]
        public void CleanData()
        {
            foreach (var temp in _tmpUs)
                new UserRepository(CommonTestClass.ConnectionString, new NoteRepository(CommonTestClass.ConnectionString, new CategoryRepository(CommonTestClass.ConnectionString)), new CategoryRepository(CommonTestClass.ConnectionString)).Delete(temp._id);
        }
    }
}
