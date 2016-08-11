using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Gem;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace GemGui.Commands
{
    /// <summary>
    /// A command for predefined process execution.
    /// </summary>
    public class ParameterizedProcessExecutorCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var processExec = parameter as ProcessExecutionDefinition;

            if (processExec != null)
            {
                s_logger.Info($"Executing process execution with parameters: {processExec.ExecutableName} {processExec.ExecutableParameters}");

                if (processExec.Equals(ProcessExecutionDefinition.EmptyExecutor))
                {
                    return;
                }

                var processInfo = new ProcessStartInfo(processExec.ExecutableName, processExec.ExecutableParameters)
                {
                    WorkingDirectory = string.IsNullOrEmpty(processExec.WorkingDirectory)
                        ? DecideWorkingFolder(processExec.ExecutableName)
                        : Utils.StripPath(processExec.WorkingDirectory), // A working directory is required not to be wrapped with quotes
                    UseShellExecute = false
                };

                // Decide if to run in administrator mode:
                if (processExec.RunElevated)
                {
                    processInfo.UseShellExecute = true;
                    processInfo.Verb = "runas";
                }

                try
                {
                    Process.Start(processInfo);

                    string elevatedText = processExec.RunElevated ? " as elevated" : string.Empty;

                    s_logger.Info($"Ran process{elevatedText}: {processExec.ExecutableName} {processExec.ExecutableParameters}");
                }
                catch (Win32Exception ex)
                {
                    s_logger.Error(ex, "While executing process, caught Win32 exception:");

                    if (ex.NativeErrorCode == 1223)
                    {
                        // User cancelled the operation:
                        MessageBox.Show("User cancelled the operation.", "GEM", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex, "While executing process, caught exception:");

                    MessageBox.Show("Some error happened running the operation, please view the log.", "GEM", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
        }

        private string DecideWorkingFolder(string executablePath)
        {
            string tempPath = Utils.StripPath(executablePath);

            if (Path.IsPathRooted(tempPath))
            {
                return Path.GetDirectoryName(tempPath);
            }

            // Fall back: The root directory of the system's partition:
            return Path.GetPathRoot(Environment.SystemDirectory);
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
