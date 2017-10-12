    using System;
using System.Collections.Generic;
using DigDesNote.Model;
using System.Data.SqlClient;


namespace DigDesNote.DataLayer.Sql
{
    public class NoteRepository : INotesRepository
    {
        private readonly String _connectionString;
        // private readonly UserRepository _userRepository;
        private readonly CategoryRepository _catRepository;

        public NoteRepository(String connectionString, CategoryRepository cat)
        {
            _connectionString = connectionString;
            // _userRepository = us;
            _catRepository = cat;
        }

        /// <summary>
        /// Добавить заметку в категорию
        /// </summary>
        /// <param name="noteId">ID заметки</param>
        /// <param name="CategoryId">ID категории</param>
        public void AddToCategory(Guid noteId, Guid CategoryId)
        {
            using (var _sqclConnection = new SqlConnection(_connectionString))
            {
                _sqclConnection.Open();
                using (var command = _sqclConnection.CreateCommand())
                {
                    command.CommandText = "insert into TRefCategoryNote (noteId, categoryId)" +
                        "values (@noteId, @categoryId)";
                    command.Parameters.AddWithValue("@noteId", noteId);
                    command.Parameters.AddWithValue("@categoryId", CategoryId);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Добавление заметки
        /// </summary>
        /// <param name="title">Заголовок</param>
        /// <param name="content">Содержимое</param>
        /// <param name="creator">Создатель</param>
        /// <returns></returns>
        public Note Create(String title, String content, Guid creator)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                Note _note = new Note()
                {
                    _id = Guid.NewGuid(),
                    _title = title,
                    _content = content,
                    _created = System.DateTime.Now,
                    _creator = creator
                };

                _sqlConnection.Open();

                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = $"insert into TNote (id, creator, title, content, createDate, updateDate) " +
                        "values(@id, @creator, @title, @content, @crateDate, @updateDate)";
                    _command.Parameters.AddWithValue("@id", _note._id);
                    _command.Parameters.AddWithValue("@creator", _note._creator);
                    _command.Parameters.AddWithValue("@title", _note._title);
                    _command.Parameters.AddWithValue("@content", _note._content);
                    _command.Parameters.AddWithValue("@crateDate", _note._created);
                    _command.Parameters.AddWithValue("@updateDate", _note._created);

                    _command.ExecuteNonQuery();
                    return _note;
                }
            }
        }

        /// <summary>
        /// Удаление заметки
        /// </summary>
        /// <param name="id">ID заметки</param>
        public void Delete(Guid id)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = $"delete from TNote where id=@noteID"; // Запрос
                    _command.Parameters.AddWithValue("@noteID", id);
                    _command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Получить информацию о заметке
        /// </summary>
        /// <param name="noteId">ID заметки</param>
        /// <returns></returns>
        public Note GetNote(Guid noteId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from TNote where id=@id";
                    command.Parameters.AddWithValue("id", noteId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read()) throw new Exception($"Заметка с id {noteId} не найдена");
                        else
                        {
                            Note note = ReaderGetNote(reader);
                            note._categories = _catRepository.GetNoteCategories(note._id);
                            return note;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Получить коллекцию заметок пользователя
        /// </summary>
        /// <param name="userID">ID заметки</param>
        /// <returns></returns>
        public IEnumerable<Note> GetUserNotes(Guid userID)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = $"select * from TNote where creator=@id";
                    _command.Parameters.AddWithValue("@id", userID);

                    using (var reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Note tmp = ReaderGetNote(reader);
                            tmp._categories = _catRepository.GetNoteCategories(tmp._id);

                            yield return tmp;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удалить заметку из категории
        /// </summary>
        /// <param name="noteId">ID заметки</param>
        /// <param name="CagegoryId">ID категории</param>
        public void RemoveFromCagegory(Guid noteId, Guid CagegoryId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = "delete from TRefCategoryNote where categoryId = @id and noteId = @noteId";
                    _command.Parameters.AddWithValue("@id", CagegoryId);
                    _command.Parameters.AddWithValue("@noteId", noteId);
                    _command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Вносит изменения в заметку
        /// </summary>
        /// <param name="id">ID заметки</param>
        /// <param name="title">Новый заголовок</param>
        /// <param name="content">Новое содержимое</param>
        /// <returns></returns>
        public Note Edit(Guid id, String title, String content)
        {
            using (var _sqlConnect = new SqlConnection(_connectionString))
            {
                _sqlConnect.Open();
                using (var _command = _sqlConnect.CreateCommand())
                {
                    _command.CommandText = "update TNote set title = @title, content = @content, updateDate = @date where id = @id";
                    _command.Parameters.AddWithValue("@id", id);
                    _command.Parameters.AddWithValue("@title", title);
                    _command.Parameters.AddWithValue("@content", content);
                    _command.Parameters.AddWithValue("@date", DateTime.Now);

                    _command.ExecuteNonQuery();

                    return GetNote(id);
                }
            }
        }

        /// <summary>
        /// Получает часть параметров
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>Id, title, content, createDate, updateDate, создателя </returns>
        private Note ReaderGetNote(SqlDataReader reader)
        {
            return new Note()
            {
                _id = reader.GetGuid(reader.GetOrdinal("id")),
                _title = reader.GetString(reader.GetOrdinal("title")),
                _content = reader.GetString(reader.GetOrdinal("content")),
                _created = reader.GetDateTime(reader.GetOrdinal("createDate")),
                _updated = reader.GetDateTime(reader.GetOrdinal("updateDate")),
                _creator = reader.GetGuid(reader.GetOrdinal("creator"))
            };
        }


        public IEnumerable<Note> GetShareUserNotes(Guid userID)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var _command = _sqlConnection.CreateCommand())
                {
                    _command.CommandText = "select note.id, note.title, note.content, note.creator, note.createDate, note.updateDate from TShare sh inner join TNote note on sh.noteId=note.id where sh.userId = @id";
                    _command.Parameters.AddWithValue("id", userID);
                    using (var reader = _command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return GetNote(reader.GetGuid(reader.GetOrdinal("id")));
                        }
                    }
                }
            }
        }

        public void Share(Guid noteId, Guid userId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = "insert into TShare (userId, noteId) " +
                        "values (@userId, @noteId)";
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@noteId", noteId);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void UnShare(Guid noteId, Guid userId)
        {
            using (var _sqlConnection = new SqlConnection(_connectionString))
            {
                _sqlConnection.Open();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from TShare where userId = @userId and noteId = @noteId";
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@noteId", noteId);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}