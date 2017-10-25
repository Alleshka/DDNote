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
    public class NoteController : ApiController
    {
        private INotesRepository _notesRepository;
        private String _connectionString = @"Data Source=DESKTOP-H4JQP0V;Initial Catalog=NoteDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public NoteController()
        {
            _notesRepository = new NoteRepository(_connectionString, new CategoryRepository(_connectionString));
        }

        /// <summary>
        /// Получить основную информацию о заметке
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/note/{id}")]
        public Note GetBasicInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Получение основной информации о заметке {id}");
            return _notesRepository.GetBasicNote(id);
        }

        /// <summary>
        /// Получить полную информацию о заметке 
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/note/{id}/full")]
        public Note GetFullInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Получение полной информации о заметке {id}");
            return _notesRepository.GetFullNote(id);
        }

        /// <summary>
        /// Получить категории заметки
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/note/{id}/categories")]
        public IEnumerable<Category> GetCategories(Guid id)
        {
            Logger.Log.Instance.Info($"Получение категорий из заметки {id}");
            return _notesRepository.GetFullNote(id)._categories;
        }

        /// <summary>
        /// Узнать кому заметка расшарена
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/note/{id}/shares")]
        public IEnumerable<Guid> GetShares(Guid id)
        {
            Logger.Log.Instance.Info($"Получение информации о пользователях, которым доступна заметка  {id}");
            return _notesRepository.GetFullNote(id)._shares;
        }

        /// <summary>
        /// Удалить заметку
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [Route("api/note/{id}/delete")]
        public void DeleteNote(Guid id)
        {
            Logger.Log.Instance.Info($"Начато удаление заметки {id}");
            _notesRepository.Delete(id);
            Logger.Log.Instance.Info($"Удаление заметки {id} завершено");
        }

        /// <summary>
        /// Обновить заметку
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="note">Структура заметки (может не содержать ID)</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/note/{id}/update")]
        public Note UpdateNote(Guid id, Note note)
        {
            Logger.Log.Instance.Info($"Внесение изменений в заметку {id}: title = {note._title}, content = {note._content}");
            return _notesRepository.Edit(id, note._title, note._content);
        }

        /// <summary>
        /// Обновить заметку
        /// </summary>
        /// <param name="note">Стукрута заметки (Должна содержать ID)</param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/note/update")]
        public Note UpdateNote([FromBody]Note note)
        {
            Logger.Log.Instance.Info($"Внесение изменений в заметку {note._id}: title = {note._title}, content = {note._content}");
            return _notesRepository.Edit(note._id, note._title, note._content);
        }

        /// <summary>
        /// Создать заметку
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/create")]
        public Note Create([FromBody]Note note)
        {
            Logger.Log.Instance.Info($"Создание заметки с параметрами: title = {note._title}, content = {note._content}, creator = {note._creator}");
            return _notesRepository.Create(note);
        }

        /// <summary>
        /// Расшарить заметку
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="userId">ID пользователя</param>
        [HttpPost]
        [Route("api/note/{id}/share/{userId}")]
        public void Share(Guid id, Guid userId)
        {
            Logger.Log.Instance.Info($"Расшарить заметку {id} пользователю {userId}");
            _notesRepository.Share(id, userId);
        }

        /// <summary>
        /// Убрать шару пользователю
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="userId">ID пользователя</param>
        [HttpPost]
        [Route("api/note/{id}/unshare/{userId}")]
        public void UnShare(Guid id, Guid userId)
        {
            Logger.Log.Instance.Info($"Убрать заметку {id} у пользователя {userId}");
            _notesRepository.UnShare(id, userId);
        }

        /// <summary>
        /// Добавить заметку в категорию
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="categoryId">ID категории</param>
        [HttpPost]
        [Route("api/note/{id}/addcategory/{categoryId}")]
        public void AddCategory(Guid id, Guid categoryId)
        {
            Logger.Log.Instance.Info($"Добавить заметку {id} в категорию {categoryId}");
            _notesRepository.AddToCategory(id, categoryId);
        }

        /// <summary>
        /// Убрать заметку из категории
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="categoryId">ID категории</param>
        [HttpPost]
        [Route("api/note/{id}/delcategory/{categoryId}")]
        public void DeleteCategory(Guid id, Guid categoryId)
        {
            Logger.Log.Instance.Info($"Убрать заметку {id} из категории {categoryId}");
            _notesRepository.RemoveFromCategory(id, categoryId);
        }
    }
}
