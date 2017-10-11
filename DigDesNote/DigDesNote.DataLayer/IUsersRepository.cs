using System;
using System.Collections.Generic;
using System.IO;
using DigDesNote.Model;

namespace DigDesNote.DataLayer
{
    public interface IUsersRepository
    {
        User Create(String login, String email, String password); // Создание пользователя 
        void Delete(Guid id); // Удаление пользователя
        User Get(Guid id); // Получить информацию о пользователе
        User Edit(Guid id, String newMail, String newPass); // Внести изменения в пользователя
    }
}