using System;
using DigDesNote.Model;

namespace DigDesNote.DataLayer
{
    public interface IUsersRepository
    {
        User Create(String login, String email, String password); // Создание пользователя 
        User Create(User user); // Создание пользователя 

        void Delete(Guid id); // Удаление пользователя

        User GetBasicUser(Guid id); // Получить основную информацию о пользователе (имя, логин, почту)
        User GetFullUser(Guid id); // Получить всю информацию о пользователе (+ все заметки, + все категории)
        User FindByLogin(String login); // Найти пользователя по логину

        User Edit(Guid id, String newMail, String newPass); // Внести изменения в пользователя
        User Edit(User user);

    }
}