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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AGLibrary.Files;
using DigDesNote.Model;
using System.Configuration;

namespace DigDesNote.UI.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String _loginPath;
        private String _domain;

        private ServiceClient _client;
        private Guid _curId; // ID пользователя, который залогинен (можно брать из файла)

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawForm(bool login)
        {
            PersonalExpand.IsEnabled = login;
            ShareExpand.IsEnabled = login;
            CategoryExpand.IsEnabled = login;
            Synchronize.IsEnabled = login;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _domain = ConfigurationManager.AppSettings["hostdomain"];
            _loginPath = ConfigurationManager.AppSettings["loginset"];
            try
            {
                _client = new ServiceClient(_domain); // Инициализация домашнего домена
                DrawForm(false);
                Login();
            }
            catch (Exception)
            {
                MessageBox.Show("Необходим повторный вход");
                System.IO.File.Delete(_loginPath);
            }
        }

        /// <summary>
        /// Выполняется при успешном входе в систему
        /// </summary>
        private void Login()
        {
            AGLibrary.Files.FileWork.ReadDataJson<LoginSet>(out LoginSet loginSet, _loginPath); // Считываем информацию о входе

            // Если не залогинились, то выводим форму входа
            if ((loginSet == null))
            {
                LoginWindow login = new LoginWindow(_client);
                if(login.ShowDialog()==true) FileWork.ReadDataJson<LoginSet>(out loginSet, _loginPath); // Считываем информацию о входе
            }

            if (loginSet != null)
            {
                GetCurLogin();
                _client.SynchronizationNotes(_curId); // Синхронизируем текущий ID
                _client.SynchronizatioCategories(_curId);
                _login.Text = _client.GetBasicUserInfo(_curId)._login; // Выводим его на форме

                DrawForm(true);

                // Чтобы форма была красивой
                ExpandReInit();

                _personalNotesList.ItemsSource = from note in _client.GetPersonalNotes(_curId) select note._title;
                _sharesNoteList.ItemsSource = from note in _client.GetSharesNotes(_curId) select note._title + " (" + _client.GetBasicUserInfo(note._creator)._login + ")";
            }
        }

        private void GetCurLogin()
        {
            FileWork.ReadDataJson<LoginSet>(out LoginSet loginSet, _loginPath);
            _curId = loginSet._userId;
        }

        private void BtnCreateNote_Click(object sender, RoutedEventArgs e)
        {
            CreateNoteWindow create = new CreateNoteWindow(_client, _curId);
            if (create.ShowDialog() == true) UpdatePersonalNotes();
        }

        // Срабатывает при развороте Expanda с личными заметками
        private void PersonalExpand_Expanded(object sender, RoutedEventArgs e)
        {
            if (_personalNotesList.ItemsSource == null) UpdatePersonalNotes();
        }

        // Срабатывает при двойном клике по личным заметкам
        private void _personalNotesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenNoteInfo(_client.GetPersonalNotes(_curId).ToList()[_personalNotesList.SelectedIndex]._id);
        }

        // Срабатывает при развороте Expanda с чужими заметками
        private void ShareExpand_Expanded(object sender, RoutedEventArgs e)
        {
            if (_sharesNoteList.ItemsSource == null)
            {
                UpdateSharesNotes();
            }
        }

        // Двойной клик по чужим заметкам
        private void _sharesNoteList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenNoteInfo(_client.GetSharesNotes(_curId).ToList()[_sharesNoteList.SelectedIndex]._id);
        }

        // Выход из УЗ
        private void ClodeProgram_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.Delete(_loginPath);
            _curId = new Guid();
            DrawForm(false);
            Login();
        }

        // Открытие категорий пользователя
        private void CategoryExpand_Expanded(object sender, RoutedEventArgs e)
        {
            if (_categoryList.Items.Count == 0)
            {
                UpdateCategories();
            }
        }

        // Нажатие на кнопку "Добавить заметку"
        private void BtnAddCat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _client.CreateCategory(new Category()
                {
                    _name = _newCatName.Text,
                    _userId = _curId
                });
                _categoryList.Items.Clear();
                UpdateCategories(); // Обновляем список категорий
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Нажатие на кнопку "Информация о заметке"
        private void NoteInfoItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_personalNotesList.Items.Count <= 0) throw new Exception("Отсутствуют заметки");
                if (_personalNotesList.SelectedIndex == -1) throw new Exception("Необходимо выбрать заметку");

                OpenNoteInfo(_client.GetPersonalNotes(_curId).ToList()[_personalNotesList.SelectedIndex]._id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Нажатие на кнопку обновления категорий
        private void UpdateCategoryItem_Click(object sender, RoutedEventArgs e)
        {
            _categoryList.Items.Clear();
            UpdateCategories();
        }

        // Нажатие на кнопку удаления заметки в контекстном меню
        private void NoteDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_personalNotesList.SelectedIndex == -1) throw new Exception("Необходимо выбрать заметку");
                _client.DeleteNote(_client.GetPersonalNotes(_curId).ToList()[_personalNotesList.SelectedIndex]._id);

                UpdatePersonalNotes();
                UpdateCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Нажатие на добавление заметки в контекстном меню
        private void NoteAddItem_Click(object sender, RoutedEventArgs e)
        {
            CreateNoteWindow create = new CreateNoteWindow(_client, _curId);
            if (create.ShowDialog() == true) UpdatePersonalNotes();
        }

        // Нажатие на кнопку информациии о зaметке в контекстном меню 
        private void ShareNoteInfoItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_personalNotesList.Items.Count == 0) throw new Exception("Отсутствуют заметки");
                if (_personalNotesList.SelectedIndex == -1) throw new Exception("Необходимо выбрать заметку");
                OpenNoteInfo(_client.GetSharesNotes(_curId).ToList()[_sharesNoteList.SelectedIndex]._id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Synchronize_Click(object sender, RoutedEventArgs e)
        {
            ExpandReInit();

            _client.SynchronizatioCategories(_curId);
            _client.SynchronizationNotes(_curId);

            UpdatePersonalNotes();
            UpdateSharesNotes();
            UpdateCategories();
        }

        // Закрытие окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileWork.ReadDataJson<LoginSet>(out LoginSet set, _loginPath);
            if ((set!=null)&&(set._statusLogin == false)) System.IO.File.Delete(_loginPath);
        }

        private void UnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_sharesNoteList.Items.Count == 0) throw new Exception("Заметки отсутствуют");
                if (_sharesNoteList.SelectedIndex == -1) throw new Exception("Необходимо выбрать заметку");
                _client.UnShareNoteToUser(_curId, _client.GetSharesNotes(_curId).ToList()[_sharesNoteList.SelectedIndex]._id);
                _client.SynchronizationNotes(_curId);
                UpdateSharesNotes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShareNotesItem_Click(object sender, RoutedEventArgs e)
        {
            AddShareList share = new AddShareList(_client, _client.GetPersonalNotes(_curId).ToList()[_personalNotesList.SelectedIndex]._id);
            if (share.ShowDialog() == true)
            {
                UpdateSharesNotes();
            }
        }

        private void CategoryNotesItem_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryWindow addCategoryWindow = new AddCategoryWindow(_client, _curId, _client.GetPersonalNotes(_curId).ToList()[_personalNotesList.SelectedIndex]._id);
            if (addCategoryWindow.ShowDialog()==true)
            {
                UpdateCategories();
            }
        }

        // -----------------------------------------------------------------------

        private void OpenNoteInfo(Guid noteId)
        {
            NoteInfo info = new NoteInfo(_client, noteId, _curId);
            if (info.ShowDialog() == true)
            {
                ExpandReInit();

                //_client.SynchronizatioCategories(_curId);
                //_client.SynchronizationNotes(_curId);

                UpdateSharesNotes();
                UpdatePersonalNotes();
                UpdateCategories();
            }
        }

        private void DelCategory(Guid id)
        {
            _client.DelCategory(id);
            _categoryList.Items.Clear();
            UpdateCategories();
        }

        // Обновление личных заметок
        private void UpdatePersonalNotes()
        {
            // _client.SynchronizationNotes(_curId);
            _personalNotesList.ItemsSource = from note in _client.GetPersonalNotes(_curId) select note._title;
        }

        // Обновление расшареных заметок
        private void UpdateSharesNotes()
        {
            // _client.SynchronizationNotes(_curId);
            _sharesNoteList.ItemsSource = from note in _client.GetSharesNotes(_curId) select note._title + " (" + _client.GetBasicUserInfo(note._creator)._login + ")";
        }

        // Обновление категорий
        public void UpdateCategories()
        {
            _categoryList.Items.Clear();

            //_client.SynchronizatioCategories(_curId);
            // Считываем все категории пользователя
            IEnumerable<Category> category = _client.GetAllCategories(_curId);

            // Проходим по каждой категории
            foreach (var k in category)
            {
                ListBox temp = new ListBox()
                {
                    ItemsSource = from note in _client.GetNotesFromCategory(k._id) select note._title,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 180
                };

                // Создаём экспандер с именем категории
                Expander expander = new Expander()
                {
                    Header = k._name + " (Заметок: " + temp.Items.Count + ")",
                    // В листбокс пихаем заголовки заметок из этой категории
                    Content = temp,
                    Width = 180,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                ContextMenu menu = new ContextMenu();
                MenuItem item1 = new MenuItem() { Header = "Удалить" }; item1.Click += (o, ea) => DelCategory(k._id);

                menu.Items.Add(item1);
                expander.ContextMenu = menu;

                temp.MouseDoubleClick += (o, ea) => OpenNoteInfo((from note in _client.GetNotesFromCategory(k._id) select note._id).ToList()[temp.SelectedIndex]);
                _categoryList.Items.Add(expander);
            }
        }

        // После перелогина очищаем форму
        private void ExpandReInit()
        {
            _personalNotesList.ItemsSource = null;
            _sharesNoteList.ItemsSource = null;
            _categoryList.ItemsSource = null;
            _categoryList.Items.Clear();

            PersonalExpand.IsExpanded = false;
            ShareExpand.IsExpanded = false;
            // CategoryExpand.IsExpanded = false;
        }


    }
}
