using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF
{
    public class ServiceClient
    {
        private readonly HttpClient _client;

        public ServiceClient(String _connectionString)
        {
            _client = new HttpClient()
            {
                BaseAddress = new System.Uri(_connectionString)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.GetCookies().Add(new CookieHeaderValue("text", "rrwr"));
        }

        /// <summary>
        /// Создание пользователя 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User CreateUser(User user)
        {
            var responce = _client.PostAsJsonAsync<User>("user", user).Result;
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<User>().Result; // Возвращаем пользователя
            }
        }

        /// <summary>
        /// Создание заметки
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public Note CreateNote(Note note)
        {
            var responce = _client.PostAsJsonAsync<Note>("note", note).Result;
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<Note>().Result; // Возвращаем заметку
            }
        }

        public Guid Login(User user)
        {
            var responce = _client.PostAsJsonAsync<User>("user/login", user).Result;
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<Guid>().Result;
            }
        }

        public IEnumerable<Note> RefreshNotes(Guid id)
        {
            var responce = _client.GetAsync($"user/{id}/notes").Result;
            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<IEnumerable<Note>>().Result;
            }
        }

        public User GetBasicUserInfo(Guid id)
        {
            var responce = _client.GetAsync($"user/{id}").Result;

            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<User>().Result;
            }
        }

        public User GetBasicUserInfo(String login)
        {
            var responce = _client.GetAsync($"user/login/{login}").Result;

            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<User>().Result;
            }
        }

        public Note GetNoteInfo(Guid id)
        {
            var responce = _client.GetAsync($"note/{id}").Result;

            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<Note>().Result;
            }
        }

        public Note UpdateNote(Note note)
        {
            var responce = _client.PutAsJsonAsync<Note>("note", note).Result;

            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<Note>().Result;
            }
        }

        public IEnumerable<Category> GetNoteCategories(Guid id)
        {
            var responce = _client.GetAsync($"note/{id}/categories").Result;

            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<IEnumerable<Category>>().Result;
            }
        }
        public IEnumerable<Category> GetAllCategories(Guid id)
        {
            var responce = _client.GetAsync($"user/{id}/categories").Result;

            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<IEnumerable<Category>>().Result;
            }
        }

        public void AddCategory(Guid categoryId, Guid noteId)
        {
            var responce = _client.PostAsync($"note/{noteId}/addcategory/{categoryId}", null).Result;
        }
        public void DelCategory(Guid categoryId, Guid noteId)
        {
            var responce = _client.PostAsync($"note/{noteId}/delcategory/{categoryId}", null).Result;
        }

        public IEnumerable<Guid> GetShares(Guid id)
        {
            var responce = _client.GetAsync($"note/{id}/shares").Result;

            if (responce.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(responce.Content.ReadAsStringAsync().Result); // Кидаем ошибку
            }
            else
            {
                return responce.Content.ReadAsAsync<IEnumerable<Guid>>().Result;
            }
        }

        public void ShareNote(String login, Guid noteId)
        {
            var responce = _client.PostAsync($"note/{noteId}/share/{GetBasicUserInfo(login)._id}", null).Result;
        }

        public void UnShareNote(Guid userId, Guid noteId)
        {
            var responce = _client.PostAsync($"note/{noteId}/unshare/{userId}", null).Result;
        }
    }
}