using System;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http;

namespace DigDesNote.API
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
    public class ModelNotValid : Exception
    {
        public ModelNotValid(string message) : base(message)
        {

        }
    }
    public class NoConnectionDB : Exception
    {
        public NoConnectionDB(String message) : base(message)
        {

        }
    }

    public class CustomExceptionAtribute : ExceptionFilterAttribute
    {
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