using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using DigDesNote.Model;
using System.ComponentModel.DataAnnotations;


namespace DigDesNote.UI.WPF
{
    public class ServiceClient
    {
        private readonly HttpClient _client;

        private IEnumerable<Note> _alluserNotes = null; // Все заметки
        private IEnumerable<Note> _personalNotes = null; // Личные заметки пользователя 
        private IEnumerable<Note> _sharesNotes = null; // Расшаренные пользователю заметки

        private IEnumerable<Category> _categories = null; // Категории пользователя

        private List<ValidationResult> result;
        private ValidationContext context;

        public ServiceClient(String _connectionString)
        {
            _client = new HttpClient()
            {
                BaseAddress = new System.Uri(_connectionString)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.GetCookies().Add(new CookieHeaderValue("text", "rrwr"));
        }
        private T ResponseParse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<T>().Result; // Возвращаем Содержимое            
            }
            else throw new Exception(response.Content.ReadAsStringAsync().Result); // Кидаем ошибку
        }

        private String GetErrors(List<ValidationResult> result)
        {
            String message = "Ошибка данных: ";
            foreach (var error in result) message += Environment.NewLine + error.ErrorMessage;
            return message;
        }
        private bool ValidateModel<T>(T model)
        {
            context = new ValidationContext(model);
            result = new List<ValidationResult>();

            if (Validator.TryValidateObject(model, context, result)) return true;
            else return false;
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        public User CreateUser(User user)
        {
            if (ValidateModel<User>(user))
            {
                var responce = _client.PostAsJsonAsync<User>("user", user).Result;
                return ResponseParse<User>(responce); // Получаем ответ
            }
            else
            {
                throw new Exception(GetErrors(result));
            }
        }

        /// <summary>
        /// Вход пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Guid Login(User user)
        {
            var response = _client.PostAsJsonAsync<User>("user/login", user).Result;
            return ResponseParse<Guid>(response); // Получаем id вошедшего пользователя
        }

        /// <summary>
        /// Получить основную информацию о пользователе по логину
        /// </summary>
        /// <param name="id">ID пользователя</param>
        public User GetBasicUserInfo(Guid id)
        {
            var response = _client.GetAsync($"user/{id}").Result;
            return ResponseParse<User>(response);
        }

        /// <summary>
        /// Синхронизировать заметки пользователя
        /// </summary>
        /// <param name="id">ID пользователя</param>
        public void SynchronizationNotes(Guid id)
        {
            var response = _client.GetAsync($"user/{id}/notes").Result;
            _alluserNotes = ResponseParse<IEnumerable<Note>>(response); // Синхронизируем все заметки пользователя

            _personalNotes = _alluserNotes.Where(x => x._creator == id).OrderBy(x => x._updated); // Получаем личные заметки пользователя
            _sharesNotes = _alluserNotes.Where(x => x._creator != id).OrderBy(x => x._updated); // Получаем расшаренные пользователю заметки
        }

        // Получиь все виды заметок
        public IEnumerable<Note> GetAllNotes() => _alluserNotes;
        public IEnumerable<Note> GetPersonalNotes() => _personalNotes;
        public IEnumerable<Note> GetSharesNotes() => _sharesNotes;

        /// <summary>
        /// Создать заметку
        /// </summary>
        /// <param name="note"></param>
        public Note CreateNote(Note note)
        {
            if (ValidateModel<Note>(note))
            {
                var response = _client.PostAsJsonAsync<Note>("note", note).Result;
                return ResponseParse<Note>(response); // Возвращаем новую заметку
            }
            else throw new Exception(GetErrors(result));
        }

        /// <summary>
        /// Получить Информацию о заметке
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Note GetBasicNoteInfo(Guid id)
        {
            var response = _client.GetAsync($"note/{id}").Result;
            return ResponseParse<Note>(response); // Получить основную информацию о заметке
        }
        /// <summary>
        /// Получить информацию о категориях из заметки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Category> GetNoteCategories(Guid id)
        {
            var response = _client.GetAsync($"note/{id}/categories").Result;
            return ResponseParse<IEnumerable<Category>>(response); // Вернуть категории из заметки
        }
        /// <summary>
        /// Вернуть информацию о том, кому доступна данная заметка
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetShares(Guid id)
        {
            var response = _client.GetAsync($"note/{id}/shares").Result;
            return ResponseParse<IEnumerable<Guid>>(response);
        }

        /// <summary>
        /// Обновить заметку
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public Note UpdateNote(Note note)
        {
            if (ValidateModel<Note>(note))
            {
                var response = _client.PutAsJsonAsync<Note>("note", note).Result;
                return ResponseParse<Note>(response); // Возвращаем новую заметку
            }
            else throw new Exception(GetErrors(result));
        }

        /// <summary>
        /// Добавить заметку в указанную категорию
        /// </summary>
        /// <param name="noteId">ID заметки</param>
        /// <param name="categoryId">ID категории</param>
        public void AddNoteToCategory(Guid noteId, Guid categoryId)
        {
            var response = _client.PostAsync($"note/{noteId}/addcategory/{categoryId}", null).Result;
        }
        /// <summary>
        /// Удалить заметку из категории
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="noteId"></param>
        public void DelNoteFromCategory(Guid noteId, Guid categoryId)
        {
           var response = _client.PostAsync($"note/{noteId}/delcategory/{categoryId}", null).Result;
        }

        /// <summary>
        /// Расшарить заметку пользователю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="noteId">ID заметки</param>
        public void ShareNoteToUser(String login, Guid noteId)
        {
            var response = _client.PostAsync($"note/{noteId}/share/{GetBasicUserInfo(login)._id}", null).Result;
        }

        /// <summary>
        /// Убрать шару от пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="noteId"></param>
        public void UnShareNoteToUser(Guid userId, Guid noteId)
        {
            var response = _client.PostAsync($"note/{noteId}/unshare/{userId}", null).Result;
        }

        /// <summary>
        /// Поличить информацию о пользователе по логину
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public User GetBasicUserInfo(String login)
        {
            var response = _client.GetAsync($"user/login/{login}").Result;
            return ResponseParse<User>(response);
        }

        /// Синхронизировать категории пользователя
        public void SynchronizatioCategories(Guid id)
        {
            var response = _client.GetAsync($"user/{id}/categories").Result;
            _categories = ResponseParse<IEnumerable<Category>>(response);
        }

        /// <summary>
        /// Польчить информацию о категориях пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Category> GetAllCategories(Guid id) => _categories;

        /// <summary>
        /// Получить все заметки из категори
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Note> GetNotesFromCategory(Guid id)
        {
            var response = _client.GetAsync($"note/category/{id}").Result;
            return ResponseParse<IEnumerable<Note>>(response);
        }

        public Category CreateCategory(Category cat)
        {
            if (ValidateModel<Category>(cat))
            {
                var response = _client.PostAsJsonAsync<Category>("category", cat).Result;
                return ResponseParse<Category>(response);
            }
            else throw new Exception(GetErrors(result));
        }

        public void DelCategory(Guid id)
        {
            var response = _client.DeleteAsync($"category/{id}").Result;
        }

        public void DeleteNote(Guid note)
        {
            var responce = _client.DeleteAsync($"note/{note}").Result;            
        }
    }
}