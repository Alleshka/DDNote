using System;
using DigDesNote.Model;
using System.Data.SqlClient;

namespace DigDesNote.DataLayer.Sql
{
    public class UserRepository : IUsersRepository
    {
        public readonly String _connectionString;
        public readonly NoteRepository _noteRepository;
        public readonly CategoryRepository _categoryRepository;

        public UserRepository(String connectionString, NoteRepository note, CategoryRepository cat)
        {
            _connectionString = connectionString;
            _noteRepository = note;
            _categoryRepository = cat;
        }

        /// <summary>
        /// Создание пользователя 
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="email">Почта</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        public User Create(String login, String email, String password)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                User user = new User()
                {
                    _id = Guid.NewGuid(),
                    _login = login,
                    _email = email,
                    _pass = password
                };

                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = $"insert into TUser (id, login, email, password) " +
                        "values(@id, @login, @email, @password)";
                    _command.Parameters.AddWithValue("@id", user._id);
                    _command.Parameters.AddWithValue("@login", user._login);
                    _command.Parameters.AddWithValue("@email", user._email);
                    _command.Parameters.AddWithValue("@password", user._pass);
                    _command.ExecuteNonQuery();

                    user._pass = null; // Чтобы не возвращать хеш пароля 

                    return user;
                }
            }
        }
        public User Create(User user)
        {
            return Create(user._login, user._email, user._pass);
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="id">ID пользователя</param>
        public void Delete(Guid id)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = $"delete from TUser where id=@id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить информацию о пользователе
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns></returns>
        public User GetBasicUser(Guid id)
        {
            using (var _sqlConnect = new SqlConnection(_connectionString))
            {
                _sqlConnect.Open();
                using (var _command = _sqlConnect.CreateCommand())
                {
                    _command.CommandText = $"select * from TUser where id=@id";
                    _command.Parameters.AddWithValue("@id", id);

                    using (var reader = _command.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        else
                        {
                            User us = new User()
                            {
                                _id = reader.GetGuid(reader.GetOrdinal("id")),
                                _login = reader.GetString(reader.GetOrdinal("login")),
                                _email = reader.GetString(reader.GetOrdinal("email"))
                            };
                            return us;
                        }
                    }
                }
            }
        }
        public User GetFullUser(Guid id)
        {
            User tmp = GetBasicUser(id);
            if (tmp == null) return null;
            tmp._categories = _categoryRepository.GetUserCategories(id);
            tmp._notes = _noteRepository.GetAllUserNotes(id); 
            return tmp;
        }

        
        /// <summary>
        /// Внесение изменения в пользователя 
        /// </summary>
        /// <param name="id">ID пользователя </param>
        /// <param name="newMail">Новая почта</param>
        /// <param name="newPass">Новый пароль</param>
        /// <returns></returns>
        public User Edit(Guid id, String newMail, String newPass)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = "update TUser set email = @newMail, password=@newPass where id=@id";
                    command.Parameters.AddWithValue("@newMail", newMail);
                    command.Parameters.AddWithValue("@newPass", newPass.GetHashCode().ToString());
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();

                    return GetBasicUser(id);
                }
            }
        }
        public User Edit(User user)
        {
            return Edit(user._id, user._email, user._pass);
        }

        public User FindByLogin(string login)
        {
            using (var _sqlConnect = new SqlConnection(_connectionString))
            {
                _sqlConnect.Open();
                using (var _command = _sqlConnect.CreateCommand())
                {
                    _command.CommandText = $"select * from TUser where login=@login";
                    _command.Parameters.AddWithValue("@login", login);

                    using (var reader = _command.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        else
                        {
                            User us = new User()
                            {
                                _id = reader.GetGuid(reader.GetOrdinal("id")),
                                _login = reader.GetString(reader.GetOrdinal("login")),
                                _email = reader.GetString(reader.GetOrdinal("email")),
                                _pass = reader.GetString(reader.GetOrdinal("password"))                             
                            };
                            return us;
                        }
                    }
                }
            }
        }
    }
}