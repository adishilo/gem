using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Gem;

namespace GemGui.Commands
{
    /// <summary>
    /// Immutable which defines how to run a process with parameters.
    /// </summary>
    public class ProcessExecutionDefinition : IEquatable<ProcessExecutionDefinition>
    {
        public static ProcessExecutionDefinition EmptyExecutor = new ProcessExecutionDefinition(string.Empty, string.Empty);

        /// <summary>
        /// Create a new <see cref="ProcessExecutionDefinition"/> object initializing it to the executable and parameters.
        /// </summary>
        /// <param name="executableName">The name of the executable.</param>
        /// <param name="executableParameters">Parameters list as given to the executable.</param>
        /// <param name="workingDirectory">The working directory for the executed process. If null or empty, uses the executable's directory.</param>
        /// <param name="runElevated">Whether to execute the command as elevated (administrative mode).</param>
        public ProcessExecutionDefinition(
            string executableName,
            LazyEvalString executableParameters,
            LazyEvalString workingDirectory = null,
            bool runElevated = false)
        {
            ExecutableName = executableName;
            ExecutableParameters = executableParameters;
            WorkingDirectory = workingDirectory;
            RunElevated = runElevated;
        }

        /// <summary>
        /// Gets the executable name to run.
        /// </summary>
        public string ExecutableName { get; }

        /// <summary>
        /// Gets the executable's parameter line.
        /// </summary>
        public LazyEvalString ExecutableParameters { get; }

        public LazyEvalString WorkingDirectory { get; }

        /// <summary>
        /// Gets whether to execute the command as elevated (administrative mode).
        /// </summary>
        public bool RunElevated { get; }

        #region IEquatable implementation

        public bool Equals(ProcessExecutionDefinition other)
        {
            if (other == null)
            {
                return false;
            }

            return
                ExecutableName.Equals(other.ExecutableName) &&
                ((string)ExecutableParameters).Equals(other.ExecutableParameters) &&
                ((string)WorkingDirectory).Equals(other.WorkingDirectory) &&
                RunElevated == other.RunElevated;
        }

        #endregion

        #region object overrides

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var exeutableDefinition = obj as ProcessExecutionDefinition;
            if (exeutableDefinition != null)
            {
                return Equals(exeutableDefinition);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return
                ExecutableName.GetHashCode() ^
                ((string)ExecutableParameters).GetHashCode() ^
                ((string)WorkingDirectory).GetHashCode() ^
                RunElevated.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// A multi-values converter for string[] -> <see cref="ProcessExecutionDefinition"/>.
    /// </summary>
    public class ProcessExecutionConverter : IMultiValueConverter
    {
        #region IMultiValueConverter implementation

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                throw new ArgumentException("Converter should have 2 parameters exactly.");
            }

            if (values.Any(value => value.Equals(DependencyProperty.UnsetValue)))
            {
                // It might be that there're still no values to handle, bail out:
                return ProcessExecutionDefinition.EmptyExecutor;
            }

            return new ProcessExecutionDefinition((string)values[0], (string)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
