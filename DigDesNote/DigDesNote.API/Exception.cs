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
                HttpResponseMessage responce = null;
                String message = "";

                // В зависимости от ситуаций
                if (actionExecutedContext.Exception is System.Data.SqlClient.SqlException)
                {
                    System.Data.SqlClient.SqlException ex = (System.Data.SqlClient.SqlException)actionExecutedContext.Exception;
                    responce = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

                    if (ex.Number == 2)
                    {
                        message = "Отсутствует подключение к базе данных";                      
                    }
                    else
                    {
                        message = "Ошибка при обращении к базе данных";
                    }
                    Logger.Log.Instance.Error(message + Environment.NewLine + actionExecutedContext.Exception.ToString());
                }

                if (actionExecutedContext.Exception is ModelNotValid)
                {
                    responce = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                    message = actionExecutedContext.Exception.Message;
                    Logger.Log.Instance.Warn(actionExecutedContext.Exception.Message);
                }

                if (actionExecutedContext.Exception is NotFoundException)
                {
                    responce = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                    message = actionExecutedContext.Exception.Message;
                    Logger.Log.Instance.Warn(actionExecutedContext.Exception.Message);
                }

                // Если необработанная ошибка
                if (responce == null)
                {
                    // Дефолтные значения
                    responce = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                    message = actionExecutedContext.Exception.Message;
                    Logger.Log.Instance.Error(actionExecutedContext.Exception.Message);
                }

                responce.Content = new StringContent(message);
                throw new HttpResponseException(responce);
            }
        }
    }
}