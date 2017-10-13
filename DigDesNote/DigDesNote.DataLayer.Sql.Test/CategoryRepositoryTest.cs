using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigDesNote.Model;
using System.Collections.Generic;
using System.Linq;

namespace DigDesNote.DataLayer.Sql.Test
{
    [TestClass]
    public class CategoryRepositoryTest
    {
        private List<User> _tmpUs;

        public CategoryRepositoryTest()
        {
            _tmpUs = new List<User>();
        }

        [TestMethod]
        /// Проверка Внесения изменений в категорию 
        public void UpdateCat()
        {
            // arrange 
            Category testCategory = CommonTestClass.GetRandCategory();
            User testUser = CommonTestClass.GetRandUser();

            String testName1 = testCategory._name;

            // act 
            CategoryRepository categoryRepository = new CategoryRepository(CommonTestClass.ConnectionString);
            UserRepository userRepository = new UserRepository(CommonTestClass.ConnectionString, new NoteRepository(CommonTestClass.ConnectionString, categoryRepository), categoryRepository);

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            testCategory = categoryRepository.Create(testUser._id, testCategory._name);

            _tmpUs.Add(testUser);
            
            Category newCat = categoryRepository.Edit(testCategory._id, testCategory._name + "666");

            // Asserts 
            Assert.AreEqual(testCategory._name, testName1);
            Assert.AreEqual(testCategory._id, newCat._id);
            Assert.AreNotEqual(newCat._name, testName1);
        }

        [TestMethod]
        public void DeleteCat()
        {
            // arrages
            Category tesCat1 = CommonTestClass.GetRandCategory();
            Category tesCat2 = CommonTestClass.GetRandCategory();

            User testUser = CommonTestClass.GetRandUser();

            // act 
            String _connect = CommonTestClass.ConnectionString;
            CategoryRepository categoryRepository = new CategoryRepository(_connect);
            UserRepository userRepository = new UserRepository(_connect, new NoteRepository(_connect, categoryRepository), categoryRepository);

            testUser = userRepository.Create(testUser._login, testUser._email, testUser._pass);
            _tmpUs.Add(testUser);

            tesCat1 = categoryRepository.Create(testUser._id, tesCat1._name);
            tesCat2 = categoryRepository.Create(testUser._id, tesCat2._name);

            int i = categoryRepository.GetUserCategories(testUser._id).Count();
            categoryRepository.Delete(tesCat1._id);
            int j = categoryRepository.GetUserCategories(testUser._id).Count();

            Assert.AreEqual(i, j + 1);
        }

        [TestCleanup]
        public void CleanData()
        {
            foreach (var temp in _tmpUs)
                new UserRepository(CommonTestClass.ConnectionString, new NoteRepository(CommonTestClass.ConnectionString, new CategoryRepository(CommonTestClass.ConnectionString)), new CategoryRepository(CommonTestClass.ConnectionString)).Delete(temp._id);
        }
    }
}