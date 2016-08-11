using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace GemGui.Commands
{
    /// <summary>
    /// Defines a command to copy a given string to the clipboard.
    /// </summary>
    public class CopyCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var copyContents = parameter as string;

            if (copyContents != null)
            {
                s_logger.Info($"Executing set clipboard with '{copyContents}'");

                try
                {
                    Clipboard.SetDataObject(copyContents, true);
                }
                catch (COMException clipboardException)
                {
                    s_logger.Error(clipboardException, $"While trying to copy information: '{copyContents}'");

                    // It turns out that the information is copied to the Clipboard after all, inspite of the exception.
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
