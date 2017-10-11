﻿using System;
using System.Collections.Generic;
using DigDesNote.Model;

namespace DigDesNote.DataLayer
{
    public interface ICategoriesRepository
    {
        Category Create(Guid userId, String name); // Создание категории 
        Category Get(Guid categoryId);
        IEnumerable<Category> GetUserCategories(Guid userId); // Получить категории пользователя
        IEnumerable<Category> GetNoteCategories(Guid noteId); // Получить категории из заметки

        void Delete(Guid categoryId); // Удаление категории 
        Category Edit(Guid id, String newName); // Редактирование категории 
    }
}