using System;
using Gem.Configuration;

namespace Gem
{
    /// <summary>
    /// Define an immutable custom executable-command relevant for an SCC environment.
    /// </summary>
    public class EnvironmentCustomCommand : IEquatable<EnvironmentCustomCommand>
    {
        public EnvironmentCustomCommand(CommandConfigElement configCommand)
            : this (configCommand.Name,
                    configCommand.ExecutablePath,
                    configCommand.Parameters,
                    configCommand.WorkingDirectory,
                    configCommand.RunElevated,
                    configCommand.Description)
        {
        }

        public EnvironmentCustomCommand(
            string name,
            string executableCommand,
            string parameters,
            string workingDirectory,
            bool runElevated,
            string description)
        {
            Utils.GuardNotNull(name, "name");
            Utils.GuardNotNull(executableCommand, "executableCommand");

            Name = name;
            ExecutableCommand = executableCommand;
            Parameters = parameters;
            WorkingDirectory = workingDirectory;
            RunElevated = runElevated;
            Description = description;
        }

        /// <summary>
        /// Gets the name of the command (user-viewable).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the path to the command's executable to run.
        /// </summary>
        public string ExecutableCommand { get; }

        /// <summary>
        /// Gets the parameters to give to the executable command.
        /// </summary>
        public string Parameters { get; }

        /// <summary>
        /// Gets the working directory to set for the executed command process.
        /// </summary>
        public string WorkingDirectory { get; }

        /// <summary>
        /// Gets whether to execute this command as elevated (administrative mode).
        /// </summary>
        public bool RunElevated { get; }

        /// <summary>
        /// Gets a human-readable description for the command.
        /// </summary>
        public string Description { get; }

        #region Equabtable<EnvironmentCustomCommand> implementation

        public bool Equals(EnvironmentCustomCommand other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Name.Equals(other.Name) &&
                ExecutableCommand.Equals(other.ExecutableCommand) &&
                Parameters.Equals(other.Parameters) &&
                WorkingDirectory.Equals(other.WorkingDirectory) &&
                RunElevated == other.RunElevated &&
                Description.Equals(other.Description);
        }

        #endregion

        #region object overrides

        public override bool Equals(object obj)
        {
            var otherCommand = obj as EnvironmentCustomCommand;
            if (otherCommand == null)
            {
                return false;
            }

            return Equals(otherCommand);
        }

        public override int GetHashCode()
        {
            return
                Name.GetHashCode() ^
                ExecutableCommand.GetHashCode() ^
                Parameters.GetHashCode() ^
                WorkingDirectory.GetHashCode() ^
                RunElevated.GetHashCode() ^
                Description.GetHashCode();
        }

        public override string ToString()
        {
            return $"[Name={Name}; ExecutableCommand={ExecutableCommand}; Parameters={Parameters}; WorkingDirectory={WorkingDirectory}; RunElevated={RunElevated}; Description={Description}]";
        }

        #endregion
    }
}
