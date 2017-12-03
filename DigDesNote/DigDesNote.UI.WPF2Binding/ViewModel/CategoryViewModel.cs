using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using DigDesNote.Model;
using DigDesNote.UI.WPF2Binding.Command;
using DigDesNote.UI.WPF2Binding.Code;
using System.Configuration;

namespace DigDesNote.UI.WPF2Binding.ViewModel
{
    /// <summary>
    /// Вьюха для категорий
    /// </summary>
    public class CategoryViewModel : BaseViewModel
    {

        ServiceClient _client;
        // Текущий пользователь
        private User _user;

        // Категории пользователя
        private ObservableCollection<Category> _userCategorues;
        public ObservableCollection<Category> UserCategories
        {
            get => _userCategorues;
            set
            {
                _userCategorues = value;
                NotifyPropertyChanged("UserCategories");
            }
        }

        // Новое имя категории
        private String _newNameCate;
        public String NewNameCat
        {
            get => _newNameCate;
            set
            {
                _newNameCate = value;
                NotifyPropertyChanged("NewNameCat");
            }
        }

        // Выбранная категория
        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                NotifyPropertyChanged("SelectedCategory");
            }
        }

        private NoteFromCategoryView _noteFromCategory;
        public NoteFromCategoryView NoteFromCategory
        {
            get => _noteFromCategory;
            set
            {
                _noteFromCategory = value;
                NotifyPropertyChanged("NoteFromCategoryView");
            }
        }

        public CategoryViewModel(User curUser)
        {
            _user = curUser; // Устанавливаем текущего пользователя
            _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
        }

        /// <summary>
        /// Создание новой категории
        /// </summary>
        public ICommand CreateCategoryCommand
        {
            get => new BaseCommand((object parametres) =>
               {
                   try
                   {
                       ServiceClient _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
                       Category temp = _client.CreateCategory(new Category()
                       {
                           _name = _newNameCate,
                           _userId = _user._id
                       });
                       _userCategorues.Add(temp);
                       NewNameCat = "";
                   }
                   catch (Exception ex)
                   {
                       System.Windows.MessageBox.Show(ex.Message);
                   }
               });
        }

        /// <summary>
        /// Удаление категории
        /// </summary>
        public ICommand DeleteCategoryCommand
        {
            get => new BaseCommand((object parametres) =>
            {
                try
                {
                    if (SelectedCategory == null) throw new Exception("Категории отсутствуют");
                    ServiceClient _client = new ServiceClient(ConfigurationManager.AppSettings["hostdomain"]);
                    _client.DelCategory(_selectedCategory._id);
                    _userCategorues.Remove(_selectedCategory);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }

        /// <summary>
        /// Загрузить категории
        /// </summary>
        public ICommand LoadCategoriesCommand
        {
            get => new BaseCommand((object par) =>
            {
                try
                {
                    if(UserCategories==null) 
                    UserCategories = new ObservableCollection<Category>(_client.GetUserCategories(_user._id));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
        }

        public ICommand ReLoadCategoriesCommand
        {
            get => new BaseCommand((object par) =>
            {
                UserCategories = null;
                LoadCategoriesCommand.Execute(null);
            });
        }

        public ICommand OpenNoteList
        {
            get => new BaseCommand((object par) =>
            {
                var k = new NoteFromCategoryView(_selectedCategory)
                {
                    Title="Заметки в категории: "
                };
                Show(k);
            });
        }
    }
}
