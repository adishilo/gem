using System;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace GemGui.Commands
{
    /// <summary>
    /// The command to exit the application.
    /// </summary>
    public class ExitCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly Window m_closeWindow;

        /// <summary>
        /// Create a new instance of <see cref="ExitCommand"/> initializing the window to be closed.
        /// </summary>
        public ExitCommand(Window closeWindow)
        {
            s_logger.Info("Executing exit.");

            m_closeWindow = closeWindow;
        }

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var window = m_closeWindow as MainWindow;

            if (window != null)
            {
                // Special treatment for the main-window of the application:
                window.OnlyHide = false;

                window.Close();
            }
            else
            {
                m_closeWindow.Close();
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
