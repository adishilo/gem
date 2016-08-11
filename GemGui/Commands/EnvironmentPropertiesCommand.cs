using System;
using System.Windows.Input;

namespace GemGui.Commands
{
    public class EnvironmentPropertiesCommand : ICommand
    {
        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
