using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TaxCalculator.ViewModel.Base
{

    public class DelegateCommand : ICommand
    {
        Action<object> _action;
        Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> action, Func<object, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }


        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
