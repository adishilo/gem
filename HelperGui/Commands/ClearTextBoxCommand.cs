using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace HelperGui.Commands
{
    /// <summary>
    /// A command to clear the text-box given at instantiation time.
    /// </summary>
    public class ClearTextBoxCommand : ICommand
    {
        private readonly TextBox m_controlledTextBox;

        public ClearTextBoxCommand(TextBox controlledTextBox)
        {
            m_controlledTextBox = controlledTextBox;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            m_controlledTextBox.Clear();
        }

        public event EventHandler CanExecuteChanged;
    }
}
