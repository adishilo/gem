using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GemGui.ViewModel;
using NLog;

namespace GemGui.Commands
{
    /// <summary>
    /// The actions that the <see cref="EditCustomFolderInfoCommand"/> command can carry.
    /// </summary>
    public enum EditCommandModes
    {
        InvokeEditMode,
        CancelEditMode,
        ApplyEdit
    }

    /// <summary>
    /// The command which is invoked when clicking a folder-info item to edit the custom information.
    /// </summary>
    public class EditCustomFolderInfoCommand : ICommand
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly SccEnvironmentsViewModel m_environmentsViewContainer;

        public EditCustomFolderInfoCommand(SccEnvironmentsViewModel environmentsViewContainer)
        {
            m_environmentsViewContainer = environmentsViewContainer;
        }

        #region ICommand implementation

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            s_logger.Info("Executing");

            var editingParameters = parameter as EnvironmentEditingDefinition;

            if (editingParameters != null)
            {
                s_logger.Info($"Executing edit custom folder info: " +
                    $"EnvironmentModel='{editingParameters.EnvironmentModel.Info}' " +
                    $"CommandMode={editingParameters.CommandMode}");

                m_environmentsViewContainer.SetEditable(editingParameters.EnvironmentModel, editingParameters.CommandMode, editingParameters.EditingTextBox);
            }
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }

    public class EditCommandParametersConverter : IMultiValueConverter
    {
        #region IMultiValueConverter implementation

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3)
            {
                throw new ArgumentException("Converter should have 3 parameters exactly.");
            }

            if (values.Any(value => value.Equals(DependencyProperty.UnsetValue)))
            {
                // It might be that there're still no values to handle, bail out:
                return ProcessExecutionDefinition.EmptyExecutor;
            }

            var environmentModel = values[1] as EnvironmentViewModel;
            if (environmentModel == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Expected type {0} for parameter 1", typeof(EnvironmentViewModel).Name));
            }

            var customTextBox = values[2] as TextBox;
            if (customTextBox == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Expected type {0} for parameter 2", typeof(TextBox)));
            }

            return new EnvironmentEditingDefinition((EditCommandModes)values[0], environmentModel, customTextBox);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
