using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace GemGui.Commands
{
    /// <summary>
    /// A command for showing the log file.
    /// </summary>
    public class ShowLogCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string logFileName = Path.Combine(executableLocation ?? string.Empty, @"logs\logfile.csv");

            s_logger.Info($"Opening log file '{logFileName}'");

            try
            {
                Process.Start(logFileName);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, $"While trying to open log file '{logFileName}' caught exception.");

                MessageBox.Show($"Could not open log file '{logFileName}'", "GEM", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
