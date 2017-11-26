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
        private List<Note> _alluserNotes = null; // Все заметки
        //private List<Note> _personalNotes = null; // Личные заметки пользователя 
        //private List<Note> _sharesNotes = null; // Расшаренные пользователю заметки

        private List<Category> _categories = null; // Категории пользователя

        private User _curUser = null;

        private List<ValidationResult> result;
        private ValidationContext context;

        public ServiceClient(String _connectionString)
        {
            _client = new HttpClient()
            {
                BaseAddress = new System.Uri(_connectionString)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            Guid userGui = ResponseParse<Guid>(response); // Получаем id вошедшего пользователя 

            // Синхронизируемся
            SynchronizatioCategories(userGui);
            SynchronizationNotes(userGui);

            return userGui;
        }

        /// <summary>
        /// Получить основную информацию о пользователе по логину
        /// </summary>
        /// <param name="id">ID пользователя</param>
        public User GetBasicUserInfo(Guid id)
        {
            if ((_curUser == null) || (_curUser._id != id))
            {
                var response = _client.GetAsync($"user/{id}").Result;
                _curUser = ResponseParse<User>(response);
            }

            return _curUser;
        }

        /// <summary>
        /// Синхронизировать заметки пользователя
        /// </summary>
        /// <param name="id">ID пользователя</param>
        public void SynchronizationNotes(Guid id)
        {
            var response = _client.GetAsync($"user/{id}/notes").Result;
            _alluserNotes = ResponseParse<IEnumerable<Note>>(response).ToList(); // Синхронизируем все заметки пользователя
        }

        // Получиь все виды заметок
        public IEnumerable<Note> GetAllNotes(Guid id) => _alluserNotes;
        public IEnumerable<Note> GetPersonalNotes(Guid id) => _alluserNotes.Where(x => x._creator == id).OrderBy(x => x._updated).ToList(); // Получаем личные заметки пользователя;
        public IEnumerable<Note> GetSharesNotes(Guid id) => _alluserNotes.Where(x => x._creator != id).OrderBy(x => x._updated).ToList();

        /// <summary>
        /// Создать заметку
        /// </summary>
        /// <param name="note"></param>
        public Note CreateNote(Note note)
        {
            if (ValidateModel<Note>(note))
            {
                var response = _client.PostAsJsonAsync<Note>("note", note).Result;
                note = ResponseParse<Note>(response); // Возвращаем новую заметку
                _alluserNotes.Add(note);

                return note;
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
            return _alluserNotes.Where(x => x._id == id).First();
            //var response = _client.GetAsync($"note/{id}").Result;
            //return ResponseParse<Note>(response); // Получить основную информацию о заметке
        }

        /// <summary>
        /// Получить информацию о категориях из заметки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>Synchro
        public IEnumerable<Category> GetNoteCategories(Guid id)
        {
            Note temp = _alluserNotes.Where(x => x._id == id).First();

            if (temp._categories == null)
            {
                var response = _client.GetAsync($"note/{id}/categories").Result;
                temp._categories = ResponseParse<IEnumerable<Category>>(response); // Вернуть категории из заметки
            }

            return temp._categories;
        }

        /// <summary>
        /// Вернуть информацию о том, кому доступна данная заметка
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetShares(Guid id)
        {
            Note temp = GetBasicNoteInfo(id);

            if (temp._shares == null)
            {
                var response = _client.GetAsync($"note/{id}/shares").Result;
                temp._shares = ResponseParse<IEnumerable<Guid>>(response);
            }

            return temp._shares;
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

                _alluserNotes.RemoveAll(x => x._id == note._id); // Удаляем элементы из коллекции 
                note = ResponseParse<Note>(response); // Возвращаем новую заметку
                _alluserNotes.Add(note); // Добавляем новую заметку
                // SynchronizationNotes(note._creator);
                return note;
            }
            else throw new Exception(GetErrors(result));
        }

        /// <summary>
        /// Добавить заметку в указанную категорию
        /// </summary>
        /// <param name="noteId">ID заметки</param>
        /// <param name="categoryId">ID категории</param>
        public String AddNoteToCategory(Guid noteId, Guid categoryId)
        {
            var response = _client.PostAsync($"note/{noteId}/addcategory/{categoryId}", null).Result;
            String repl = ResponseParse<String>(response);

            Note temp = _alluserNotes.Where(x => x._id == noteId).First();
            // Обновляем категории 
            if (temp._categories != null)
            {
                temp._categories = null;
                // GetNoteCategories(temp._id);
            }
            return repl;
        }

        /// <summary>
        /// Удалить заметку из категории
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="noteId"></param>
        public String DelNoteFromCategory(Guid noteId, Guid categoryId)
        {
           var response = _client.PostAsync($"note/{noteId}/delcategory/{categoryId}", null).Result;
           String repl = ResponseParse<String>(response);

            Note temp = _alluserNotes.Where(x => x._id == noteId).First();
            // Обновляем категории 
            if (temp._categories != null)
            {
                temp._categories = null;
                // GetNoteCategories(temp._id);
            };
            return repl;
        }

        /// <summary>
        /// Расшарить заметку пользователю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="noteId">ID заметки</param>
        public String ShareNoteToUser(String login, Guid noteId)
        {
            var response = _client.PostAsync($"note/{noteId}/share/{GetBasicUserInfo(login)._id}", null).Result;   
            String repl = ResponseParse<String>(response);

            Note temp = _alluserNotes.Where(x => x._id == noteId).First();
            temp._shares = null;

            return repl;
        }

        /// <summary>
        /// Убрать шару от пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="noteId"></param>
        public String UnShareNoteToUser(Guid userId, Guid noteId)
        {
            var response = _client.PostAsync($"note/{noteId}/unshare/{userId}", null).Result;

            String repl = ResponseParse<String>(response);

            Note temp = _alluserNotes.Where(x => x._id == noteId).First();
            temp._shares = null;

            return repl;
        }

        /// <summary>
        /// Поличить информацию о пользователе по логину
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public User GetBasicUserInfo(String login)
        {
            if (login != _curUser._login)
            {
                var response = _client.GetAsync($"user/login/{login}").Result;
                _curUser = ResponseParse<User>(response);
            }

            return _curUser;
        }

        /// Синхронизировать категории пользователя
        public void SynchronizatioCategories(Guid id)
        {
            var response = _client.GetAsync($"user/{id}/categories").Result;
            _categories = ResponseParse<IEnumerable<Category>>(response).ToList();
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
                Category temp = ResponseParse<Category>(response);
                _categories.Add(temp);
                return temp;
            }
            else throw new Exception(GetErrors(result));
        }

        public String DelCategory(Guid id)
        {
            var response = _client.DeleteAsync($"category/{id}").Result;
            String repl = ResponseParse<String>(response);

            _categories.RemoveAll(x => x._id == id); // Удаляем категории из памяти

            return repl;
        }

        public String DeleteNote(Guid note)
        {
            var response = _client.DeleteAsync($"note/{note}").Result;
            String answ = ResponseParse<String>(response);
            _alluserNotes.RemoveAll(x => x._id == note); // Удаляем элементы из коллекции
            return answ;
        }

        public Category RenameCategory(Guid id, String name)
        {
            var response = _client.PutAsJsonAsync<String>($"category/{id}", name).Result;
            Category tmp = ResponseParse<Category>(response);
            _categories.RemoveAll(x => x._id == id); _categories.Add(tmp);
            return tmp;
        }
    }
}