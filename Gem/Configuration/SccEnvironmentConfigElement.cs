using System.Configuration;

namespace Gem.Configuration
{
    /// <summary>
    /// A configuration element maintaining information persistent for an SCC environment.
    /// </summary>
    public class SccEnvironmentConfigElement : ConfigurationElement
    {
        public const char CommandIdSeparator = ';';

        #region Field names

        private const string c_folder = "folder";
        private const string c_sccInfo = "sccInfo";
        private const string c_customInfo = "customInfo";
        private const string c_isViewable = "isViewable";
        private const string c_customCommandIds = "customCommandIds";

        #endregion

        /// <summary>
        /// Gets or sets the absolute path to the folder identified with this SCC environment.
        /// </summary>
        [ConfigurationProperty(c_folder, IsRequired = true)]
        public string Folder
        {
            get
            {
                return (string)this[c_folder];
            }

            set
            {
                this[c_folder] = value;
            }
        }

        /// <summary>
        /// Gets or sets SCC-related information identified with this environment.
        /// </summary>
        [ConfigurationProperty(c_sccInfo, IsRequired = true)]
        public string SccInfo
        {
            get
            {
                return (string)this[c_sccInfo];
            }

            set
            {
                this[c_sccInfo] = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom info inserted by the user for this SCC environment.
        /// </summary>
        [ConfigurationProperty(c_customInfo, IsRequired = true)]
        public string CustomInfo
        {
            get
            {
                return (string)this[c_customInfo];
            }

            set
            {
                this[c_customInfo] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to view the environment, depending on the view.
        /// </summary>
        [ConfigurationProperty(c_isViewable, IsRequired = false, DefaultValue = true)]
        public bool IsViewable
        {
            get
            {
                return (bool)this[c_isViewable];
            }

            set
            {
                this[c_isViewable] = value;
            }
        }

        /// <summary>
        /// Gets or sets a comma-separated list of command IDs for custom command of the configured CustomCommands collection
        /// to be used for this environment.
        /// </summary>
        [ConfigurationProperty(c_customCommandIds, IsRequired = false, DefaultValue = "")]
        public string CustomCommandIds
        {
            get
            {
                return (string)this[c_customCommandIds];
            }

            set
            {
                this[c_customCommandIds] = value;
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
