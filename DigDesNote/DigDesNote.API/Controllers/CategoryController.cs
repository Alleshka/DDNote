using System;
using System.Collections.Generic;
using System.Web.Http;
using DigDesNote.Model;
using DigDesNote.DataLayer;
using DigDesNote.DataLayer.Sql;
using System.ComponentModel.DataAnnotations;

namespace DigDesNote.API.Controllers
{
    /// <summary>
    /// Управление пользователями
    /// </summary>
    public class CategoryController : ApiController
    {
        private String _connectionString = @"Data Source=DESKTOP-H4JQP0V;Initial Catalog=NoteDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private ICategoriesRepository _categoriesRepository;

        private List<ValidationResult> result;
        private ValidationContext context;

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
        [CustomExceptionAtribute]
        [Authorize]
        [Route("api/category/{id}")]
        public Category GetCategoryInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получить информацию о категории {id}");
            Category category = _categoriesRepository.Get(id);

            if (category == null) throw new NotFoundException($"Категория с id = {id} не найдена");
            else return category;

        }

        /// <summary>
        /// Создать категорию
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomExceptionAtribute]
        [Route("api/category")]
        public Category CreateCategory([FromBody]Category category)
        {
            Logger.Log.Instance.Info($"Попытка пользователя {category._userId} создать категорю {category._name}");

            result = new List<ValidationResult>();
            var context = new ValidationContext(category);

            if (!Validator.TryValidateObject(category, context, result, true))
            {
                String message = "Не удалось создать категорию. Указаны не все параметры: ";
                foreach (var error in result) message += Environment.NewLine + error.ErrorMessage;
                throw new ModelNotValid(message);
            }
            else
            {
                Category cat = _categoriesRepository.Create(category._userId, category._name);
                Logger.Log.Instance.Info($"Категория {category._name} была создана пользователем {cat._userId}. id = {cat._id}");
                return cat;
            }
        }

        /// <summary>
        /// Обновить категорию
        /// </summary>
        [HttpPut]
        [CustomExceptionAtribute]
        [Route("api/category/{id}")]
        public Category UpdateCategory(Guid id, [FromBody]String name)
        {
            Logger.Log.Instance.Info($"Попытка переименовать категорию {id} в {name}");

            if (_categoriesRepository.Get(id) == null) throw new NotFoundException($"Категория {id} не найдена");

            Category cat = _categoriesRepository.Edit(id, name);
            Logger.Log.Instance.Info($"Категория {id} была переименована в {name}");
            return cat;

        }

        ///// <summary>
        ///// Обновить категорию
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="newName"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("api/category/{id}/{newName}")]
        //public Category UpdateCategory(Guid id, String newName)
        //{
        //    Logger.Log.Instance.Info($"Попытка переименовать категорию {id} в {newName}");
        //    return _categoriesRepository.Edit(id, newName);
        //}

        /// <summary>
        /// Удалить категорию
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [CustomExceptionAtribute]
        [Route("api/category/{id}")]
        public void DeleteCategory(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка удалить категорию {id}");
            if (_categoriesRepository.Get(id) == null) throw new NotFoundException($"Категория {id} не найдена");
            _categoriesRepository.Delete(id);
            Logger.Log.Instance.Info($"Категория {id} была удалена");
        }
    }
}
