using System;
using System.Collections.Generic;
using DigDesNote.Model;

namespace DigDesNote.DataLayer
{
    public interface  INotesRepository
    {
        Note Create(String title, String content, Guid creator); // Создание заметки 
        void Delete(Guid id); // Удаление заметки 
        IEnumerable<Note> GetUserNotes(Guid userID); // Получение заметок пользователя, в которых он создатель 
        IEnumerable<Note> GetShareUserNotes(Guid userID); // Получение заметок, которые пользователю расшарили 
        IEnumerable<Note> GetAllUserNotes(Guid userID); // Получить все заметки пользователя
        Note GetNote(Guid noteId); // Получение информации о заметке по id
        Note Edit(Guid id, String title, String content); // Редактирование заметки 
        void Share(Guid noteId, Guid userId); // Шарим заметку
        void UnShare(Guid noteId, Guid userId); // Убираем шару
        void AddToCategory(Guid noteId, Guid CategoryId); // Добавить заметку в категорию
        void RemoveFromCategory(Guid noteId, Guid CategoryId); // Удалить заметку из категории
    }
}