using System;
using System.Collections.Generic;
using System.Linq;
using DigDesNote.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using System.IO;
using DigDesNote.UI.WPF2Binding.Code;
using DigDesNote.UI.WPF2Binding.Model;

namespace DigDesNote.UI.WPF2Binding.Code
{
    public class ServiceClient
    {
        private HttpClient _client;
        public ServiceClient(String _connectionString)
        {
            _client = new HttpClient()
            {
                BaseAddress = new System.Uri(_connectionString)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region validate
        private List<ValidationResult> result;
        private ValidationContext context;

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
        #endregion

        public void StartProgram()
        {
            if (!Directory.Exists("adm")) Directory.CreateDirectory("adm");
        }

        #region users
        /// <summary>
        /// Вход пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Guid Login(User user)
        {
            if ((user._login == "")) throw new Exception("Введите логин");
            if ((user._pass == "")) throw new Exception("Введите пароль");

            user._pass = user._pass.GetHashCode().ToString();
            var response = _client.PostAsJsonAsync<User>("user/login", user).Result;
            Guid userGui = ResponseParse<Guid>(response); // Получаем id вошедшего пользователя 

            return userGui;
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User RegisterUser(User user)
        {
            if (ValidateModel<User>(user))
            {
                user._pass = user._pass.GetHashCode().ToString();
                var responce = _client.PostAsJsonAsync<User>("user", user).Result;
                return ResponseParse<User>(responce); // Получаем ответ
            }
            else
            {
                throw new Exception(GetErrors(result));
            }
        }

        public User GetBasicUserInfo(Guid id)
        {
            var response = _client.GetAsync($"user/{id}").Result;
            User _curUser = ResponseParse<User>(response);

            return _curUser;
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

        public IEnumerable<NoteCategoriesModel> GetUserCategories(Guid id)
        {
            var response = _client.GetAsync($"user/{id}/categories").Result;
            return ResponseParse<IEnumerable<NoteCategoriesModel>>(response).ToList();
        }
        #endregion

        #region notes
        public List<NoteModel> GetPersonalNotes(Guid id)
        {
            var response = _client.GetAsync($"user/{id}/notes/personal").Result;
            List<NoteModel> notes = ResponseParse<IEnumerable<NoteModel>>(response).OrderBy(x => x._id).ToList();
            return notes;
        }

        /// <summary>
        /// Получить категории из заметки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <summary>
        /// Получить информацию о категориях из заметки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>Synchro
        public List<NoteCategoriesModel> GetNoteCategories(Guid id)
        {
            var response = _client.GetAsync($"note/{id}/categories").Result;
            var _categories = ResponseParse<IEnumerable<NoteCategoriesModel>>(response).OrderBy(x => x._id).ToList(); // Вернуть категории из заметки

            return _categories;
        }
        /// <summary>
        /// Получить список пользователей, которым доступна заметка
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<User> GetNoteShares(Guid id)
        {
            var response = _client.GetAsync($"note/{id}/shares").Result;
            var guids = ResponseParse<IEnumerable<Guid>>(response).OrderBy(x => x);
            List<User> _shares = new List<User>();
            foreach (var k in guids) _shares.Add(GetBasicUserInfo(k));
            return _shares;
        }
        public NoteModel UpdateNote(NoteModel note)
        {
            if (ValidateModel<Note>(note))
            {
                var response = _client.PutAsJsonAsync<NoteModel>("note", note).Result;
                note = ResponseParse<NoteModel>(response); // Возвращаем новую заметку
                return note;
            }
            else throw new Exception(GetErrors(result));
        }

        public String AddNoteToCategory(Guid noteId, Guid categoryId)
        {
            var response = _client.PostAsync($"note/{noteId}/addcategory/{categoryId}", null).Result;
            return ResponseParse<String>(response);
        }

        public String DelNoteToCategory(Guid noteId, Guid categoryId)
        {
            var response = _client.PostAsync($"note/{noteId}/delcategory/{categoryId}", null).Result;
            return ResponseParse<String>(response);
        }

        /// <summary>
        /// Расшарить заметку пользователю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="noteId">ID заметки</param>
        public String ShareNoteToUser(String login, Guid noteId)
        {
            var response = _client.PostAsync($"note/{noteId}/share/{GetBasicUserInfo(login)._id}", null).Result;
            return ResponseParse<String>(response);
        }

        /// <summary>
        /// Убрать шару от пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="noteId"></param>
        public String UnShareNoteToUser(Guid userId, Guid noteId)
        {
            var response = _client.PostAsync($"note/{noteId}/unshare/{userId}", null).Result;

            return ResponseParse<String>(response);
        }
        #endregion

        #region categories

        #endregion

        #region shares
        #endregion

    }
}