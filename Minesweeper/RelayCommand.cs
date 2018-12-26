using System;
using System.Windows.Input;

namespace Minesweeper
{
    public class RelayCommand : ICommand
    {
        private readonly Action _Command;

        public RelayCommand(Action command)
        {
            _Command = command;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _Command();
        }
    }
}