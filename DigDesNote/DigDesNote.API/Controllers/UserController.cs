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
    public class UserController : ApiController
    {
        private IUsersRepository _userRepository;
        private String _connectionString = @"Data Source=DESKTOP-H4JQP0V;Initial Catalog=NoteDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
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
            return _userRepository.GetBasicUser(id);
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
            return _userRepository.GetFullUser(id);
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
            return _userRepository.GetFullUser(id)._categories;
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
            return _userRepository.GetFullUser(id)._notes;
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user/create")]
        public User CreateUser([FromBody]User user)
        {
            return _userRepository.Create(user);
        }

        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [Route("api/user/{id}/delete")]
        public void DeleteUser(Guid id)
        {
            _userRepository.Delete(id);
        }


        [HttpPut]
        [Route("api/user/{id}/update")]
        public User UpdateUser(Guid id, [FromBody]User user)
        {
            return _userRepository.Edit(id, user._email, user._pass);
        }

        [HttpPut]
        [Route("api/user/update")]
        public User UpdateUser([FromBody]User user)
        {
            return _userRepository.Edit(user._id, user._email, user._pass);
        }
    }
}
