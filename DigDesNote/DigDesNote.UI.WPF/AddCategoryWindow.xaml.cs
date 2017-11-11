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

        private readonly Guid _userID; // Текущий ID
        private readonly Guid _noteID; // Текущая заметка

        private List<Category> _allcategory;
        private List<Category> _curCategory;

        bool redact; // Флаг указывающий на редактирование

        public AddCategoryWindow(ServiceClient client, Guid user, Guid note)
        {
            InitializeComponent();

            _client = client;

            _userID = user;
            _noteID = note;

            redact = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshCategories();
            redact = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = redact;
        }

        public void RefreshCategories()
        {
            // Считываем категории пользователя 
            _allcategory = _client.GetAllCategories(_userID).OrderBy(x => x._id).ToList();
            _curCategory = _client.GetNoteCategories(_noteID).OrderBy(x => x._id).ToList();

            AddedCategory.ItemsSource = from cat in _curCategory select cat._name;
            AllCategory.ItemsSource = from cat in _allcategory select cat._name;

            redact = true;
        }

        // Добавить категорию
        private void AddCategoryItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AllCategory.Items.Count == 0) throw new Exception("Необходимо создать категорию");
                if (AllCategory.SelectedIndex == -1) throw new Exception("Необходимо выбрать категорию");

                _client.AddNoteToCategory(_noteID, _allcategory[AllCategory.SelectedIndex]._id);
                RefreshCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Удалить категорию
        private void DelCategoryItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AddedCategory.Items.Count == 0) throw new Exception("Отсутствуют добавленные категории");
                if (AddedCategory.SelectedIndex == -1) throw new Exception("Небходимо выбрать категорию");
                _client.DelNoteFromCategory(_noteID, _curCategory[AddedCategory.SelectedIndex]._id);
                RefreshCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
