using System;
using System.Windows.Input;
using GemGui.ViewModel;
using NLog;

namespace GemGui.Commands
{
    /// <summary>
    /// The command to refresh the structure of the managed SCC folders.
    /// </summary>
    public class RefreshStructureCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly GemViewModel m_viewModel;

        public RefreshStructureCommand(GemViewModel viewModel)
        {
            m_viewModel = viewModel;
        }

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            s_logger.Info("Executing");

            await m_viewModel.RefreshFoldersStructure(true);
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
