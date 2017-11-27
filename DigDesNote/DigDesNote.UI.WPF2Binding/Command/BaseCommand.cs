using System;
using System.Windows.Input;

namespace DigDesNote.UI.WPF2Binding.Command
{
    public class BaseCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter = null)
        {
            return this.canExecute == null || this.CanExecute(parameter);
        }

        public void Execute(object parameter = null)
        {
            this.execute(parameter);
        }

        public BaseCommand(Action<object> execute, Func<object, bool> canexecute = null)
        {
            this.canExecute = canexecute;
            this.execute = execute;
        }
    }
}