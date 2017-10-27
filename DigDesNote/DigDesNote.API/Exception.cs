using System;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Web.Http;

namespace DigDesNote.API
{
    public class NoFoundException : Exception
    {
        public NoFoundException(string message) : base(message)
        {

        }
    }
    public class ModelNotValid : Exception
    {
        public ModelNotValid(string message) : base(message)
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

                /// Дефолтные значения
                responce = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                message = actionExecutedContext.Exception.Message;

                /// В зависимости от ситуаций
                if (actionExecutedContext.Exception.GetType() == typeof(System.Data.SqlClient.SqlException))
                {
                    responce = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                    message = "Ошибка при обращении к базе данных";
                }
                if (actionExecutedContext.Exception.GetType() == typeof(ModelNotValid))
                {
                    responce = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                    message = actionExecutedContext.Exception.Message;
                }
                if (actionExecutedContext.Exception.GetType() == typeof(NoFoundException))
                {
                    responce = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
                    message = actionExecutedContext.Exception.Message;
                }

                responce.Content = new StringContent(message);
                Logger.Log.Instance.Error(actionExecutedContext.Exception.Message);
                throw new HttpResponseException(responce);
            }
        }
    }
}