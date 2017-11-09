using System;
using System.Collections.Generic;
using System.Web.Http;
using DigDesNote.Model;
using DigDesNote.DataLayer;
using DigDesNote.DataLayer.Sql;
using System.ComponentModel.DataAnnotations;
using DigDesNote.API.Filter;

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
        public User GetBasicInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получения основной информации о пользователе {id}");
            User user = _userRepository.GetBasicUser(id);
            if (user == null) throw new NotFoundException($"Пользователь {id} не найден");
            else return user;
        }

        [HttpGet]
        [Route("api/user/login/{login}")]
        public User GetBasicInfo(String login)
        {
            Logger.Log.Instance.Info($"Попытка получения основной информации о пользователе {login}");
            User user = _userRepository.FindByLogin(login);
            if (user == null) throw new NotFoundException($"Пользователь {login} не найден");
            else return user;
        }

        /// <summary>
        /// Получить всю информацию о пользователе (основная + основная информация о заметках пользователя + информация о категориях пользователя)
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/{id}/full")]
        public User GetFullInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получения полной информации о пользователе {id}");
            User user = _userRepository.GetFullUser(id);
            if (user == null) throw new NotFoundException($"Пользователь {id} не найден");
            else return user;
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
        public User CreateUser([FromBody]User user)
        {
            Logger.Log.Instance.Info($"Попытка создания пользователя c параметрами: login = {user._login}, email = {user._email};");

            if (!ModelState.IsValid)
            {
                String message = "Невозможно создать пользователя: ";

                foreach (var values in ModelState.Values)
                {
                    foreach (var error in values.Errors) message += Environment.NewLine + error.ErrorMessage;
                }

                throw new ModelNotValid(message);
            }
            else
            {
                User tmp = _userRepository.Create(user._login, user._email, user._pass);
                Logger.Log.Instance.Info($"Пользователь {tmp._id} создан");
                return tmp;
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
        [Route("api/user/{id}")]
        public User UpdateUser(Guid id, [FromBody]User user)
        {
            Logger.Log.Instance.Info($"Попытка внесения изменений в пользователя с id = {id}");
            if (_userRepository.GetBasicUser(id) == null) throw new NotFoundException($"Пользователь {id} не найден");

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
                User tmp =  _userRepository.Edit(id, user._email, user._pass);
                Logger.Log.Instance.Info($"Пользователь {id} успешно изменён");
                return tmp;
            }
        }

        [HttpPost]
        [Route("api/user/login")]
        public Guid Login([FromBody]User user)
        {
            User findUser = _userRepository.FindByLogin(user._login);
            if (findUser == null) throw new NotFoundException("Пользователь не найден");

            if (findUser._pass == user._pass) return findUser._id;
            else throw new Exception("Пароль указан неверно");
        }
    }
}
