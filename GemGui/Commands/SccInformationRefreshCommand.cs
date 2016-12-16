using System;
using System.Windows.Input;
using NLog;

namespace GemGui.Commands
{
    /// <summary>
    /// A command to refresh the SCC information in the background
    /// </summary>
    public class SccInformationRefreshCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            s_logger.Info("Trying to refresh SCC information");
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
