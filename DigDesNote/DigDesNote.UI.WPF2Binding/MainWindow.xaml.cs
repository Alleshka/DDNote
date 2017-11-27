using System.Windows;
using DigDesNote.UI.WPF2Binding.ViewModel;

namespace DigDesNote.UI.WPF2Binding
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowModel model = new MainWindowModel();
            this.DataContext = model;
        }
    }
}
