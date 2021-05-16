using HandyControl.Data;
using System;
using System.Windows.Input;

namespace OGM
{
    public class RelayCommand : ICommand
    {
        private Predicate<FunctionEventArgs<object>> _canExecute;
        private Action<FunctionEventArgs<object>> _execute;

        public RelayCommand(Predicate<FunctionEventArgs<object>> canExecute, Action<FunctionEventArgs<object>> execute)
        {
            this._canExecute = canExecute;
            this._execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((FunctionEventArgs<object>)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((FunctionEventArgs<object>)parameter);
        }
    }
}
