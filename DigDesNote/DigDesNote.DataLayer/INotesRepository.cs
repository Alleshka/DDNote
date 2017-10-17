using System;
using System.Collections.Generic;
using DigDesNote.Model;

namespace DigDesNote.DataLayer
{
    public interface  INotesRepository
    {
        Note Create(String title, String content, Guid creator); // Создание заметки 
        Note Create(Note note); // Создание заметки 

        void Delete(Guid id); // Удаление заметки 

        IEnumerable<Note> GetUserNotes(Guid userID); // Получение заметок пользователя, в которых он создатель 
        IEnumerable<Note> GetShareUserNotes(Guid userID); // Получение заметок, которые пользователю расшарили 
        IEnumerable<Note> GetAllUserNotes(Guid userID); // Получить все заметки пользователя
        IEnumerable<Guid> GetShares(Guid noteId); // Получает список пользователей, которым расшарена заметка

        Note GetBasicNote(Guid noteId); // Получение основной информации (заголовок, содержимое, id создателя)
        Note GetFullNote(Guid noteId); // Получение всей информации (основная + список категорий + шар)

        Note Edit(Guid id, String title, String content); // Редактирование заметки 

        void Share(Guid noteId, Guid userId); // Шарим заметку
        void UnShare(Guid noteId, Guid userId); // Убираем шару
        void AddToCategory(Guid noteId, Guid CategoryId); // Добавить заметку в категорию
        void RemoveFromCategory(Guid noteId, Guid CategoryId); // Удалить заметку из категории
    }
}