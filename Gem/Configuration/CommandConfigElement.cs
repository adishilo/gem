using System.Configuration;

namespace Gem.Configuration
{
    /// <summary>
    /// Configuration element for maintaining an executable command.
    /// </summary>
    public class CommandConfigElement : ConfigurationElement
    {
        #region Field Names

        private const string c_name = "name";
        private const string c_executablePath = "executablePath";
        private const string c_parameters = "parameters";
        private const string c_workingDirectory = "workingDirectory";
        private const string c_runElevated = "runElevated";
        private const string c_description = "description";

        #endregion

        /// <summary>
        /// Gets or sets the name of the command (user-viewable).
        /// </summary>
        [ConfigurationProperty(c_name, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this[c_name];
            }

            set
            {
                this[c_name] = value;
            }
        }

        /// <summary>
        /// Gets or sets the path to the command's executable to run.
        /// </summary>
        [ConfigurationProperty(c_executablePath, IsRequired = true)]
        public string ExecutablePath
        {
            get
            {
                return (string)this[c_executablePath];
            }

            set
            {
                this[c_executablePath] = value;
            }
        }

        /// <summary>
        /// Gets or sets the parameters to give to the executable command.
        /// </summary>
        [ConfigurationProperty(c_parameters, IsRequired = false, DefaultValue = "")]
        public string Parameters
        {
            get
            {
                return (string)this[c_parameters];
            }

            set
            {
                this[c_parameters] = value;
            }
        }

        /// <summary>
        /// Gets or sets the working directory to set for the executed command process.
        /// </summary>
        [ConfigurationProperty(c_workingDirectory, IsRequired = false, DefaultValue = "")]
        public string WorkingDirectory
        {
            get
            {
                return (string)this[c_workingDirectory];
            }

            set
            {
                this[c_workingDirectory] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to execute this command as elevated (administrative mode).
        /// </summary>
        [ConfigurationProperty(c_runElevated, IsRequired = false, DefaultValue = false)]
        public bool RunElevated
        {
            get
            {
                return (bool)this[c_runElevated];
            }

            set
            {
                this[c_runElevated] = value;
            }
        }

        /// <summary>
        /// Gets or sets a human-readable description for the command.
        /// </summary>
        [ConfigurationProperty(c_description, IsRequired = false, DefaultValue = "")]
        public string Description
        {
            get
            {
                return (string)this[c_description];
            }

            set
            {
                this[c_description] = value;
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
