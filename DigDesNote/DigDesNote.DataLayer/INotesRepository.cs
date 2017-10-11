﻿using System;
using System.Collections.Generic;
using DigDesNote.Model;

namespace DigDesNote.DataLayer
{
    public interface  INotesRepository
    {
        Note Create(String title, String content, Guid creator); // Создание заметки 
        void Delete(Guid id); // Удаление заметки 

        IEnumerable<Note> GetUserNotes(Guid userID); // Получение заметок пользователя 
        Note GetNote(Guid noteId); // Получение информации о заметке по id

        Note Edit(Guid id, String title, String content); // Редактирование заметки 

        void AddToCategory(Guid noteId, Guid CategoryId); // Добавить заметку в категорию
        void RemoveFromCagegory(Guid noteId, Guid CagegoryId); // Удалить заметку из категории
    }
}