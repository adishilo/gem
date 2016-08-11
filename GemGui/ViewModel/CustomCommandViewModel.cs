using Gem;

namespace GemGui.ViewModel
{
    /// <summary>
    /// A view model for an <see cref="EnvironmentCustomCommand"/> object.
    /// </summary>
    public class CustomCommandViewModel
    {
        public CustomCommandViewModel(EnvironmentCustomCommand customCommand)
        {
            Name = customCommand.Name;
            ExecutableCommand = customCommand.ExecutableCommand;
            Parameters = customCommand.Parameters;
            WorkingDirectory = customCommand.WorkingDirectory;
            RunElevated = customCommand.RunElevated;
            Description = customCommand.Description;
        }

        /// <summary>
        /// Gets the name of the command (user-viewable).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the path to the command's executable to run.
        /// </summary>
        public string ExecutableCommand { get; set; }

        /// <summary>
        /// Gets the parameters to give to the executable command.
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Gets the working directory to set for the executed command process.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets whether to execute this command as elevated (administrative mode).
        /// </summary>
        public bool RunElevated { get; set; }

        /// <summary>
        /// Gets a human-readable description for the command.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Convert the command view model back to its model counterpart, creating a copy.
        /// </summary>
        /// <returns></returns>
        public static implicit operator EnvironmentCustomCommand(CustomCommandViewModel commandViewModel)
        {
            return new EnvironmentCustomCommand(
                commandViewModel.Name,
                commandViewModel.ExecutableCommand,
                commandViewModel.Parameters,
                commandViewModel.WorkingDirectory,
                commandViewModel.RunElevated,
                commandViewModel.Description);
        }
    }
}