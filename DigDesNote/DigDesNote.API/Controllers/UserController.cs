using System;
using System.Collections.Generic;
using System.Web.Http;
using DigDesNote.Model;
using DigDesNote.DataLayer;
using DigDesNote.DataLayer.Sql;
using System.ComponentModel.DataAnnotations;
using DigDesNote.API.Models;

namespace DigDesNote.API.Controllers
{
    /// <summary>
    /// Управление пользователями
    /// </summary>
    [CustomExceptionAtribute]
    public class UserController : ApiController
    {
        private IUsersRepository _userRepository;
        private String _connectionString = @"Data Source=DESKTOP-H4JQP0V;Initial Catalog=NoteDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private List<ValidationResult> result;
        private ValidationContext context;

        public UserController()
        {
            _userRepository = new UserRepository(_connectionString, new NoteRepository(_connectionString, new CategoryRepository(_connectionString)), new CategoryRepository(_connectionString));
        }

        /// <summary>
        /// Получить основную информацию о пользователе (id, логин, почту)
        /// </summary>
        /// <param name="id">Уникальные идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/{id}")]
        public BasicUser GetBasicInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получения основной информации о пользователе {id}");
            User user = _userRepository.GetBasicUser(id);
            if (user == null) throw new NotFoundException($"Пользователь {id} не найден");
            else return new BasicUser(user);
        }

        /// <summary>
        /// Получить всю информацию о пользователе (основная + основная информация о заметках пользователя + информация о категориях пользователя)
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/{id}/full")]
        public FullUser GetFullInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получения полной информации о пользователе {id}");
            User user = _userRepository.GetFullUser(id);
            if (user == null) throw new NotFoundException($"Пользователь {id} не найден");
            else return new FullUser(user);
        }

        /// <summary>
        /// Получить информацию о категориях пользователя пользователя
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/{id}/categories")]
        public IEnumerable<Category> GetUserCategories(Guid id)
        {
            Logger.Log.Instance.Info($"Получение категорий пользователе {id}");
            if (_userRepository.GetBasicUser(id) == null) throw new NotFoundException($"Пользователь {id} не найден");
            else return _userRepository.GetFullUser(id)._categories;
        }

        /// <summary>
        /// Получить основную информацию о всех заметках пользователя
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/{id}/notes")]
        public IEnumerable<Note> GetUserNotes(Guid id)
        {
            Logger.Log.Instance.Info($"Получение заметок пользователя {id};");
            if (_userRepository.GetBasicUser(id) == null) throw new NotFoundException($"Пользователь {id} не найден");
            return _userRepository.GetFullUser(id)._notes;
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user")]
        public BasicUser CreateUser([FromBody]CreateUser user)
        {
            Logger.Log.Instance.Info($"Попытка создания пользователя c параметрами: login = {user._login}, email = {user._email};");

            context = new ValidationContext(user);
            result = new List<ValidationResult>();

            if (!Validator.TryValidateObject(user, context, result))
            {
                String message = "Невозможно создать пользователя. Указаны не все параметры: ";
                foreach (var error in result) message += Environment.NewLine + error.ErrorMessage;
                throw new ModelNotValid(message);
            }
            else
            {
                User tmp =  _userRepository.Create(user._login, user._email, user._password);
                Logger.Log.Instance.Info($"Пользователь {tmp._id} создан");
                return new BasicUser(tmp);
            }
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [Route("api/user/{id}")]
        public void DeleteUser(Guid id)
        {
            Logger.Log.Instance.Info($"Начато удаление пользователя с id = {id};");
            _userRepository.Delete(id);
            Logger.Log.Instance.Info($"Удаление пользователя с id = {id} успешно завершено");
        }


        ///// <summary>
        ///// Редактирование пользователя
        ///// </summary>
        ///// <param name="id">ID пользователя</param>
        ///// <param name="user">Новое содержимое</param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("api/user/{id}")]
        //public User UpdateUser(Guid id, [FromBody]User user)
        //{
        //        Logger.Log.Instance.Info($"Внесение изменений в пользователя с id = {id}");
        //        return _userRepository.Edit(id, user._email, user._pass);
        //}

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        /// <param name="user">Содержимое для смены. Необходимо указание id</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/user")]
        public BasicUser UpdateUser([FromBody]EditUser user)
        {
            Logger.Log.Instance.Info($"Попытка внесения изменений в пользователя с id = {user._id}");
            if (_userRepository.GetBasicUser(user._id) == null) throw new NotFoundException($"Пользователь {user._id} не найден");

            context = new ValidationContext(user);
            result = new List<ValidationResult>();


            if (!Validator.TryValidateObject(user, context, result))
            {
                String message = "Невозможно изменить пользователя. Указаны не все параметры: ";
                foreach (var error in result) message += Environment.NewLine + error.ErrorMessage;
                throw new ModelNotValid(message);
            }
            else
            {
                User tmp =  _userRepository.Edit(user._id, user._email, user._password);
                Logger.Log.Instance.Info($"Пользователь {user._id} успешно изменён");
                return new BasicUser(tmp);
            }
        }
    }
}
