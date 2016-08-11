using System.Configuration;

namespace Gem.Configuration
{
    public class GemConfigSection : ConfigurationSection
    {
        public const string SectionName = "GemApplication";

        #region Configuration field names

        private const string c_searchRootFolder = "searchRootFolder";
        private const string c_maxSearchDepth = "maxSearchDepth";
        private const string c_customCommands = "CustomCommands";
        private const string c_sccEnvironments = "SccEnvironments";

        #endregion

        /// <summary>
        /// Gets or sets the configuration for the root folder from which to search for SCC connected folders.
        /// </summary>
        [ConfigurationProperty(c_searchRootFolder, IsRequired = false, DefaultValue = null)]
        public string SearchRootFolder
        {
            get
            {
                return (string)this[c_searchRootFolder];
            }

            set
            {
                this[c_searchRootFolder] = value;
            }
        }

        /// <summary>
        /// Gets or sets the configuration for the maximum folder depth for the search.
        /// </summary>
        [ConfigurationProperty(c_maxSearchDepth, IsRequired = false, DefaultValue = 3)]
        public int MaxSearchDepth
        {
            get
            {
                return (int)this[c_maxSearchDepth];
            }

            set
            {
                this[c_maxSearchDepth] = value;
            }
        }

        /// <summary>
        /// Gets the collection of the custom commands defined for SCC environments to use.
        /// </summary>
        [ConfigurationProperty(c_customCommands, IsRequired = false)]
        [ConfigurationCollection(
            typeof(CommandsConfigCollection),
            AddItemName = "Command")]
        public CommandsConfigCollection CustomCommands => (CommandsConfigCollection)this[c_customCommands];

        public override bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Gets the collection of persisted SCC environments.
        /// </summary>
        [ConfigurationProperty(c_sccEnvironments, IsRequired = false)]
        [ConfigurationCollection(
            typeof(SccEnvironmentsConfigCollection),
            AddItemName = "Add",
            RemoveItemName = "Remove",
            ClearItemsName = "Clear")]
        public SccEnvironmentsConfigCollection SccEnvironments => (SccEnvironmentsConfigCollection)this[c_sccEnvironments];
    }
}
