using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DigDesNote.Model;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для AddCategoryWindow.xaml
    /// </summary>
    public partial class AddCategoryWindow : Window
    {
        private readonly ServiceClient _client;
        private readonly Guid _userID;
        private readonly Guid _noteID;

        private List<Category> _allcategory;
        private List<Category> _curCategory;

        bool redact;

        public AddCategoryWindow(ServiceClient client, Guid user, Guid note)
        {
            InitializeComponent();
            _client = client;
            _userID = user;
            _noteID = note;

            _curCategory = _client.GetNoteCategories(_noteID).OrderBy(x => x._id).ToList(); // Считываем категории из заметки
            _allcategory = _client.GetAllCategories(_userID).OrderBy(x => x._id).ToList(); // Считываем все категории

            redact = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshCategories();
            redact = false;
        }

        private void RefreshCategories()
        {
            _curCategory = _client.GetNoteCategories(_noteID).OrderBy(x => x._id).ToList(); // Считываем категории из заметки
            _allcategory = _client.GetAllCategories(_userID).OrderBy(x => x._id).ToList(); // Считываем все категории

            AddedCategory.ItemsSource = from cat in _curCategory select cat._name;
            AllCategory.ItemsSource = from cat in _allcategory select cat._name;
            redact = true;
        }

        /// <summary>
        /// Добавить категорию к заметке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllCategory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_allcategory.Count != 0)
            {
                Guid id = _allcategory[AllCategory.SelectedIndex]._id; // Достаём ID категории 
                _client.AddCategory(id, _noteID); // Добавляем заметку в категорию 

                _curCategory.Add(_allcategory[AllCategory.SelectedIndex]); // Добавляем в окно вывода
                RefreshCategories();
            }
        }

        private void AddedCategory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_curCategory.Count != 0)
            {
                Guid id = _curCategory[AddedCategory.SelectedIndex]._id; // Достаём ID категории 
                _client.DelCategory(id, _noteID); // Добавляем заметку в категорию 

                _curCategory.RemoveAt(AddedCategory.SelectedIndex); // Удаляем из окна вывода
                RefreshCategories();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = redact;
        }
    }
}
