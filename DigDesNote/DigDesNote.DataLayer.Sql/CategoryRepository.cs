using System;
using System.Collections.Generic;
using DigDesNote.Model;
using System.Data.SqlClient;

namespace DigDesNote.DataLayer.Sql
{
    public class CategoryRepository : ICategoriesRepository
    {
        private readonly String _connectionSring;

        public CategoryRepository(String connectionString)
        {
            _connectionSring = connectionString;
        }

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="userId">ID пользователя, который создаёт категорию</param>
        /// <param name="name">Имя категории</param>
        /// <returns></returns>
        public Category Create(Guid userId, string name)
        {
            using (var _sqlConnect = new SqlConnection(_connectionSring))
            {
                Category _cat = new Category()
                {
                    _id = Guid.NewGuid(),
                    _name = name,
                    _userId = userId
                };
                _sqlConnect.Open();

                using (var _command = _sqlConnect.CreateCommand())
                {
                    _command.CommandText = $"insert into TCategory (id, userId, Name) values(@id, @userId, @name)"; // Добавляем запрос
                    _command.Parameters.AddWithValue("@id", _cat._id);
                    _command.Parameters.AddWithValue("@userId", userId);
                    _command.Parameters.AddWithValue("@name", _cat._name);

                    _command.ExecuteNonQuery(); // Выполняем запрос
                    _cat._userId = userId;
                    return _cat;
                }

            }
        }
        
        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="category">Категория, которая содержит в себе название категории и ID пользователя</param>
        /// <returns></returns>
        public Category Create(Category category)
        {
            return Create(category._userId, category._name);
        }

        /// <summary>
        /// Получение информации о категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Category</returns>
        public Category Get(Guid categoryId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionSring))
            {
                _sqlConnection.Open();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from TCategory where id = @id";
                    command.Parameters.AddWithValue("@id", categoryId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        else
                        {
                            return new Category()
                            {
                                _id = reader.GetGuid(reader.GetOrdinal("id")),
                                _name = reader.GetString(reader.GetOrdinal("name")),
                                _userId = reader.GetGuid(reader.GetOrdinal("userId"))
                            };  
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        /// <param name="categoryId">ID удаляемой категории</param>
        public void Delete(Guid categoryId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionSring))
            {
                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = "delete from TCategory where id=@categoryId"; // Запрос
                    _command.Parameters.AddWithValue("@categoryId", categoryId);
                    _command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить список категорий для пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<Category> GetUserCategories(Guid userId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionSring))
            {
                _sqlConnection.Open();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = $"select id, name from TCategory where userId = @id";
                    command.Parameters.AddWithValue("@id", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Category()
                            {
                                _name = reader.GetString(reader.GetOrdinal("name")),
                                _id = reader.GetGuid(reader.GetOrdinal("id")),
                                _userId = userId
                            };
                        }
                    }
                }
                _sqlConnection.Close();
            }
        }
        /// <summary>
        /// Получить список категорий для заметки
        /// </summary>
        /// <param name="noteId">ID заметки</param>
        /// <returns>Коллекцию категорий</returns>
        public IEnumerable<Category> GetNoteCategories(Guid noteId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionSring))
            {
                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = $"select cat.id, cat.Name, cat.userId from TNote note inner join TRefCategoryNote RCN on note.id=RCN.noteId inner join TCategory cat on RCN.categoryId = cat.id where note.id = @id";
                    _command.Parameters.AddWithValue("id", noteId); 
                    using (var reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Category()
                            {
                                _id = reader.GetGuid(reader.GetOrdinal("id")),
                                _name = reader.GetString(reader.GetOrdinal("name")),
                                _userId = reader.GetGuid(reader.GetOrdinal("userId"))
                            };
                        }
                    }
                }
                _sqlConnection.Close();
            }
        }
        /// <summary>
        /// Редактировать категорию
        /// </summary>
        /// <param name="id">ID категории</param>
        /// <param name="newName">Новое имя категории</param>
        /// <returns></returns>
        public Category Edit(Guid id, String newName)
        {
            using (var _sqlConnection = new SqlConnection(_connectionSring))
            {
                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = "update TCategory set name=@name where id = @id";
                    _command.Parameters.AddWithValue("@id", id);
                    _command.Parameters.AddWithValue("@name", newName);
                    _command.ExecuteNonQuery();
                    return Get(id);
                }
            }
        }
    }
}