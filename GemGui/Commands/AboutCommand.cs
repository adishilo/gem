using System;
using System.Windows.Input;
using NLog;

namespace GemGui.Commands
{
    public class AboutCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private static AboutWindow m_aboutDialog;

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            s_logger.Info("Executing");

            if (IsDialogOpen)
            {
                m_aboutDialog.Focus();

                return;
            }

            IsDialogOpen = true;
            m_aboutDialog = new AboutWindow();
            m_aboutDialog.ShowDialog();
            IsDialogOpen = false;
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        /// <summary>
        /// Gets whether the 'About' dialog is now open.
        /// </summary>
        public bool IsDialogOpen { get; private set; }

        public void Close()
        {
            m_aboutDialog?.Close();
        }
    }
}
