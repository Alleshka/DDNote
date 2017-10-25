using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DigDesNote.Model;
using DigDesNote.DataLayer;
using DigDesNote.DataLayer.Sql;

namespace DigDesNote.API.Controllers
{
    /// <summary>
    /// Управление пользователями
    /// </summary>
    public class CategoryController : ApiController
    {
        private String _connectionString = @"Data Source=DESKTOP-H4JQP0V;Initial Catalog=NoteDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private ICategoriesRepository _categoriesRepository;

        public CategoryController()
        {
            _categoriesRepository = new CategoryRepository(_connectionString);
        }

        /// <summary>
        /// Получить информацию о категории
        /// </summary>
        /// <param name="id">ID категории</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/category/{id}")]
        public Category GetCategoryInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Получить информацию о категории {id}");
            return _categoriesRepository.Get(id);
        }

        /// <summary>
        /// Создать категорию
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category")]
        public Category CreateCategory([FromBody]Category category)
        {
            Logger.Log.Instance.Info($"Пользователь {category._userId} создал категорю {category._name}");
            return _categoriesRepository.Create(category);
        }

        /// <summary>
        /// Создать категорию
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="id">ID пользователя</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/category/{name}/{id}")]
        public Category CreateCategory(String name, Guid id)
        {
            Logger.Log.Instance.Info($"Пользователь {id} создал категорю {name}");
            return _categoriesRepository.Create(id, name);
        }

        /// <summary>
        /// Обновить категорию
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/category/{id}")]
        public Category UpdateCategory(Category category)
        {
            Logger.Log.Instance.Info($"Попытка переименовать категорию {category._id} в {category._name}");
            return _categoriesRepository.Edit(category._id, category._name);
        }

        /// <summary>
        /// Обновить категорию
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/category/{id}/{newName}")]
        public Category UpdateCategory(Guid id, String newName)
        {
            Logger.Log.Instance.Info($"Попытка переименовать категорию {id} в {newName}");
            return _categoriesRepository.Edit(id, newName);
        }

        /// <summary>
        /// Удалить категорию
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [Route("api/category/{id}")]
        public void DeleteCategory(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка удалить категорию {id}");
            _categoriesRepository.Delete(id);
            Logger.Log.Instance.Info($"Категория {id} была удалена");
        }
    }
}
