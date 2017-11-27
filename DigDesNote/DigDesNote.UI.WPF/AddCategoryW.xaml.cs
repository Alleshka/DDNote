using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для AddCategoryW.xaml
    /// </summary>
    public partial class AddCategoryW : Window
    {
        ServiceClient _client; // 
        Guid _noteId;
        Guid _userId;

        private bool redact = false;

        public AddCategoryW(ServiceClient client, Guid userId, Guid noteId)
        {
            InitializeComponent();
            _client = client;
            _noteId = noteId;
            _userId = userId;
        }

        private void ListBoxCategory_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshListBox();
        }

        private void AddRemoveCat(Guid id, bool check)
        {
            // Click срабатывает после нажатия, поэтому статус нажатия инвертируется
            // Если удалить из категории
            if (check == false)
            {
                _client.DelNoteFromCategory(_noteId, id);
            }
            // Если добавить в категорию
            else
            {
                _client.AddNoteToCategory(_noteId, id);
            }

            redact = true;
            RefreshListBox();
        }
        private void RefreshListBox()
        {
            ListBoxCategory.Items.Clear();
            // Загружаем все категории пользователя
            foreach (var k in _client.GetAllCategories(_userId))
            {
                // Создаём новый checkBox
                CheckBox temp = new CheckBox()
                {
                    Content = k._name
                };
                // Если среди категорий заметок есть такой, жмём его в true
                if (_client.GetNoteCategories(_noteId).Any(x => x._id == k._id)) temp.IsChecked = true;
                else temp.IsChecked = false;
                // Вешаем на него действие
                temp.Click += (o, ea) => AddRemoveCat(k._id, (bool)temp.IsChecked);

                ListBoxCategory.Items.Add(temp);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = redact;
        }
    }
}
