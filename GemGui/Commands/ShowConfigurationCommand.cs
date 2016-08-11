using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Gem.Configuration;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace GemGui.Commands
{
    /// <summary>
    /// A command for showing the configuration file.
    /// </summary>
    public class ShowConfigurationCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private Boolean m_allowEnter = true;

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!m_allowEnter)
            {
                return;
            }

            m_allowEnter = false;

            var configFileName = GemConfigManager.UserConfigFileName;

            s_logger.Info($"Opening configuration file '{configFileName}'");

            MessageBox.Show($"Now opening GEM Configuration file '{configFileName}'." +
                            $"{Environment.NewLine} - It is recommended to save a copy before changing this file." +
                            $"{Environment.NewLine} - Be sure to restart GEM for the changes to take effect.", "GEM", MessageBoxButton.OK, MessageBoxImage.Asterisk);

            try
            {
                m_allowEnter = true;
                Process.Start(configFileName);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, $"While trying to open configuration file '{configFileName}' caught exception.");

                MessageBox.Show($"Could not open log file '{configFileName}'", "GEM", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
