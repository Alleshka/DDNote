using System;
using System.ComponentModel;
using System.Windows;
using DigDesNote.UI.WPF.View;

namespace DigDesNote.UI.WPF.ViewModel
{
    public class BaseViewModel : DependencyObject, INotifyPropertyChanged
    {
        private BaseView _view = null;

        // Заголовок окна
        public String Title
        {
            get => (String)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BaseViewModel), new PropertyMetadata(""));

        /// <summary>
        /// Методы вызываемые при закрытии окна
        /// </summary>
        protected virtual void Closed()
        {

        }

        /// <summary>
        /// Вызывается для закрытия окна
        /// </summary>
        /// <returns></returns>
        protected void Close(object parametres)
        {
            _view.Close();
            _view = null;
        }
        protected void Show(BaseViewModel viewModel)
        {
            viewModel._view = new BaseView
            {
                DataContext = viewModel
            };

            viewModel._view.Closed += (sender, e) => Closed();
            viewModel._view.Show();
        }

        protected bool ShowDialog(BaseViewModel viewModel)
        {
            viewModel._view = new BaseView
            {
                DataContext = viewModel
            };

            viewModel._view.Closed += (sender, e) => Closed();
            return (bool)viewModel._view.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}