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
    /// 
    /// </summary>
    public class NoteController : ApiController
    {
        private INotesRepository _notesRepository;
        private String _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["_connectionString"].ConnectionString;
        //private String _connectionString = @"Data Source=DigDesNoteDb.mssql.somee.com;Initial Catalog=DigDesNoteDb;Integrated Security=False;User ID=Alleshka_SQLLogin_1;Password=atxj8cdh9i;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private List<ValidationResult> result;
        private ValidationContext context;

        /// <summary>
        /// 
        /// </summary>
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
        [CustomExceptionAtribute]
        [Route("api/note/{id}")]
        public Note GetBasicInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получения основной информации о заметке {id}");

            Note note = _notesRepository.GetBasicNote(id);
            if (note == null) throw new NotFoundException($"Заметка с id = {id} не найдена");
            else return note;
        }

        /// <summary>
        /// Получить полную информацию о заметке 
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <returns></returns>
        [HttpGet]
        [CustomExceptionAtribute]
        [Route("api/note/{id}/full")]
        public Note GetFullInfo(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получения полной информации о заметке {id}");
            Note note = _notesRepository.GetBasicNote(id);
            if(note==null) throw new NotFoundException($"Заметка с id = {id} не найдена");
            return _notesRepository.GetFullNote(id);
        }

        /// <summary>
        /// Получить категории заметки
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <returns></returns>
        [HttpGet]
        [CustomExceptionAtribute]
        [Route("api/note/{id}/categories")]
        public IEnumerable<Category> GetCategories(Guid id)
        {
            Logger.Log.Instance.Info($"Получение категорий из заметки {id}");
            if (_notesRepository.GetBasicNote(id) == null) throw new NotFoundException($"Заметка {id} не найдена");
            return _notesRepository.GetFullNote(id)._categories;
        }

        /// <summary>
        /// Узнать кому заметка расшарена
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <returns></returns>
        [HttpGet]
        [CustomExceptionAtribute]
        [Route("api/note/{id}/shares")]
        public IEnumerable<Guid> GetShares(Guid id)
        {
            Logger.Log.Instance.Info($"Получение информации о пользователях, которым доступна заметка {id}");
            if (_notesRepository.GetBasicNote(id) == null) throw new NotFoundException($"Заметка {id} не найдена");
            return _notesRepository.GetFullNote(id)._shares;
        }

        /// <summary>
        /// Удалить заметку
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [CustomExceptionAtribute]
        [Route("api/note/{id}")]
        public void DeleteNote(Guid id)
        {
            Logger.Log.Instance.Info($"Начато удаление заметки {id}");
            if (_notesRepository.GetBasicNote(id) == null) throw new NotFoundException($"Заметка {id} не найдена");
            _notesRepository.Delete(id);
            Logger.Log.Instance.Info($"Удаление заметки {id} завершено");
        }

        ///// <summary>
        ///// Обновить заметку
        ///// </summary>
        ///// <param name="id">ID заметки</param>
        ///// <param name="note">Структура заметки (может не содержать ID)</param>
        ///// <returns></returns>
        //[HttpPut]
        //[CustomExceptionAtribute]
        //[Route("api/note/{id}")]
        //public Note UpdateNote(Guid id, UpdateNote note)
        //{
        //    Logger.Log.Instance.Info($"Внесение изменений в заметку {id}: title = {note._title}, content = {note._content}");
        //    return _notesRepository.Edit(id, note._title, note._content);
        //}

        /// <summary>
        /// Обновить заметку
        /// </summary>
        /// <param name="note">Стукрута заметки (Должна содержать ID)</param>
        /// <returns></returns>
        [HttpPut]
        [CustomExceptionAtribute]
        [Route("api/note")]
        public Note UpdateNote([FromBody]Note note)
        {
            Logger.Log.Instance.Info($"Попытка внесения изменений в заметку {note._id}: title = {note._title}, content = {note._content}");
            if (_notesRepository.GetBasicNote(note._id) == null) throw new NotFoundException($"Заметка {note._id} не найдена");

            context = new ValidationContext(note);
            result = new List<ValidationResult>();

            if (!Validator.TryValidateObject(note, context, result))
            {
                String message = "Невозможно изменить заметку. Указаны не все параметры: ";
                foreach (var error in result) message += Environment.NewLine + error.ErrorMessage;
                throw new ModelNotValid(message);
            }
            else
            {
                Note nt = _notesRepository.Edit(note._id, note._title, note._content);
                Logger.Log.Instance.Info($"Заметка {note._id} была изменена");
                return nt;
            }
        }

        /// <summary>
        /// Создать заметку
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomExceptionAtribute]
        [Route("api/note")]
        public Note Create([FromBody]Note note)
        {
            Logger.Log.Instance.Info($"Попытка создания заметки с параметрами: title = {note._title}, content = {note._content}, creator = {note._creator}");

            context = new ValidationContext(note);
            result = new List<ValidationResult>();

            if (!Validator.TryValidateObject(note, context, result))
            {
                String message = "Невозможно создать заметку. Указаны не все параметры: ";
                foreach (var error in result) message += Environment.NewLine + error.ErrorMessage;
                throw new ModelNotValid(message);
            }
            else
            {
                Note nt = _notesRepository.Create(note._title, note._content, note._creator);
                Logger.Log.Instance.Info($"Заметка {nt._id} создана");
                return nt;
            }
        }

        /// <summary>
        /// Расшарить заметку
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="userid">ID пользователя</param>
        [HttpPost]
        [CustomExceptionAtribute]
        [Route("api/note/{id}/share/{userid}")]
        public void Share(Guid id, Guid userid)
        {
            Logger.Log.Instance.Info($"Расшарить заметку {id} пользователю {userid}");
            Note temp = _notesRepository.GetBasicNote(id);

            if ( temp == null) throw new NotFoundException($"Заметка {id} не найдена");
            if (temp._creator == userid) throw new Exception($"Нельзя поделиться заметку самому себе");

            _notesRepository.Share(id, userid);
            Logger.Log.Instance.Info($"Заметка {id} расшарена пользователю {userid}");
        }

        /// <summary>
        /// Убрать шару пользователю
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        [HttpPost]
        [CustomExceptionAtribute]
        [Route("api/note/{id}/unshare/{userid}")]
        public void UnShare(Guid id, Guid userid)
        {
            Logger.Log.Instance.Info($"Убрать заметку {id} у пользователя {userid}");
            if (_notesRepository.GetBasicNote(id) == null) throw new NotFoundException($"Заметка {id} не найдена");
            _notesRepository.UnShare(id, userid);
            Logger.Log.Instance.Info($"Заметка {id} больше недоступна пользователю {userid}");
        }

        /// <summary>
        /// Добавить заметку в категорию
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="categoryId">ID категории</param>
        [HttpPost]
        [CustomExceptionAtribute]
        [Route("api/note/{id}/addcategory/{categoryId}")]
        public void AddCategory(Guid id, Guid categoryId)
        {
            Logger.Log.Instance.Info($"Добавить заметку {id} в категорию {categoryId}");
            if (_notesRepository.GetBasicNote(id) == null) throw new NotFoundException($"Заметка {id} не найдена");
            _notesRepository.AddToCategory(id, categoryId);
            Logger.Log.Instance.Info($"Заметка {id} добавлена в категорию {categoryId}");
        }

        /// <summary>
        /// Убрать заметку из категории
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="categoryId">ID категории</param>
        [HttpPost]
        [CustomExceptionAtribute]
        [Route("api/note/{id}/delcategory/{categoryId}")]
        public void DeleteCategory(Guid id, Guid categoryId)
        {
            Logger.Log.Instance.Info($"Убрать заметку {id} из категории {categoryId}");
            if (_notesRepository.GetBasicNote(id) == null) throw new NotFoundException($"Заметка {id} не найдена");
            _notesRepository.RemoveFromCategory(id, categoryId);
            Logger.Log.Instance.Info($"Заметку {id} удалена из категории {categoryId}");
        }

        /// <summary>
        /// Получить все заметки из категории
        /// </summary>
        /// <param name="id">ID категории</param>
        /// <returns></returns>
        [HttpGet]
        [CustomExceptionAtribute]
        [Route("api/note/category/{id}")]
        public IEnumerable<Note> NotesInCategory(Guid id)
        {
            Logger.Log.Instance.Info($"Попытка получить заметки из категории {id}");
            return _notesRepository.GetNoteFromCategory(id);
        }
    }
}
