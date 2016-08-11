using System;
using System.Windows;
using System.Windows.Input;
using GemGui.ViewModel;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace GemGui.Commands
{
    public class ConfigureCommand : ICommand, IDialogRelatedCommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly GemViewModel m_mainViewModel;

        private ConfigureWindow m_configureDialog;

        public ConfigureCommand(GemViewModel mainViewModel)
        {
            m_mainViewModel = mainViewModel;
        }

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            s_logger.Info("Executing");

            if (IsDialogOpen)
            {
                m_configureDialog.Focus();

                return;
            }

            IsDialogOpen = true;
            var configModelView = new ConfigurationViewModel(m_mainViewModel.CustomCommands, m_mainViewModel.SccEnvironmentsModel.EnvironmentsView)
            {
                MaxSearchDepth = m_mainViewModel.MaxSearchDepth,
                RootFolderForSearch = m_mainViewModel.RootFolder
            };

            m_configureDialog = new ConfigureWindow(configModelView);

            var configureResult = m_configureDialog.ShowDialog();

            if (configureResult.HasValue && configureResult.Value)
            {
                m_mainViewModel.MaxSearchDepth = m_configureDialog.ViewModel.MaxSearchDepth;

                // If the root folder changed, and was not empty, alert the user he might loose environments' information:
                MessageBoxResult userDecisionForNewRoot;
                if (string.IsNullOrEmpty(m_mainViewModel.RootFolder))
                {
                    userDecisionForNewRoot = MessageBoxResult.Yes;
                }
                else
                {
                    if (m_mainViewModel.RootFolder.Equals(m_configureDialog.ViewModel.RootFolderForSearch))
                    {
                        userDecisionForNewRoot = MessageBoxResult.No;
                    }
                    else
                    {
                        userDecisionForNewRoot = MessageBox.Show(
                            m_configureDialog,
                            "The environments' root folder has changed. If you choose to continue, environments' description will be lost. Continue?",
                            "Root Folder Changed",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
                    }
                }

                if (userDecisionForNewRoot == MessageBoxResult.Yes)
                {
                    m_mainViewModel.RootFolder = m_configureDialog.ViewModel.RootFolderForSearch;

                    await m_mainViewModel.RefreshFoldersStructure();
                }
            }

            IsDialogOpen = false;
        }

        public event EventHandler CanExecuteChanged;

        #endregion

        #region IDialogRelatedCommand implementation

        public bool IsDialogOpen { get; private set; }

        public bool CloseDialog()
        {
            if (m_configureDialog != null && IsDialogOpen)
            {
                var userChoice = MessageBox.Show(
                    m_configureDialog,
                    "Cancelling all configuration changes. Are you sure?",
                    "Configuration Closing",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (userChoice == MessageBoxResult.No)
                {
                    return false;
                }

                m_configureDialog.Close();
            }

            return true;
        }

        #endregion
    }
}
