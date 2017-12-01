using System;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http;

namespace DigDesNote.API
{
    /// <summary>
    /// Исключение, если объект не найден
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NotFoundException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Исключение, если модель не валидна
    /// </summary>
    public class ModelNotValid : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ModelNotValid(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Исключение, если нет доступа к БД
    /// </summary>
    public class NoConnectionDB : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NoConnectionDB(String message) : base(message)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomExceptionAtribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null)
            {
                HttpResponseMessage response = null;
                String message = "";

                // В зависимости от ситуаций
                if (actionExecutedContext.Exception is System.Data.SqlClient.SqlException)
                {
                    System.Data.SqlClient.SqlException ex = (System.Data.SqlClient.SqlException)actionExecutedContext.Exception;
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

                    switch (ex.Number)
                    {
                        case 2:
                            {
                                message = "Отсутствует подключение к базе данных";
                                break;
                            }
                        case 2627:
                            {
                                message = "Такой элемент уже сушествует";
                                break;
                            }
                        default:
                            {
                                message = "Необработанная ошибка при обращении к базе данных" + Environment.NewLine + actionExecutedContext.Exception.ToString();
                                break;
                            }
                    }
  
                    Logger.Log.Instance.Error(message);
                }

                if (actionExecutedContext.Exception is ModelNotValid)
                {
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                    message = actionExecutedContext.Exception.Message;
                    Logger.Log.Instance.Warn(actionExecutedContext.Exception.Message);
                }

                if (actionExecutedContext.Exception is NotFoundException)
                {
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                    message = actionExecutedContext.Exception.Message;
                    Logger.Log.Instance.Warn(actionExecutedContext.Exception.Message);
                }

                // Если необработанная ошибка
                if (response == null)
                {
                    // Дефолтные значения
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                    message = actionExecutedContext.Exception.Message;
                    Logger.Log.Instance.Error(actionExecutedContext.Exception.Message);
                }

                response.Content = new StringContent(message);
                throw new HttpResponseException(response);
            }
        }
    }
}