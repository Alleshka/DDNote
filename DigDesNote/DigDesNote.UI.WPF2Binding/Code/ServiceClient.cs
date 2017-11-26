using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigDesNote.Model;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

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
    }
}