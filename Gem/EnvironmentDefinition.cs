using System.Collections.Generic;
using System.Linq;
using Gem.Configuration;
using NLog;

namespace Gem
{
    /// <summary>
    /// A definition for an SCC environment. This defines a monitor on a local folder.
    /// </summary>
    public class EnvironmentDefinition
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Maps the name of a custom command to the command's definition.
        /// </summary>
        private readonly Dictionary<string, EnvironmentCustomCommand> m_customCommands = new Dictionary<string, EnvironmentCustomCommand>();

        public EnvironmentDefinition(ISccProvider sccProvider)
        {
            SccProvider = sccProvider;
            IsViewable = true;
        }

        /// <summary>
        /// Gets the related SCC provider.
        /// </summary>
        public ISccProvider SccProvider { get; }

        /// <summary>
        /// Gets the monitored folder name.
        /// </summary>
        public string FolderName => SccProvider.LocalFolderName;

        /// <summary>
        /// Gets SCC-related information about this environment, in human readable fashion.
        /// </summary>
        public string FolderSccInformationPresentable => SccProvider.FolderContentTitlePresentable;

        /// <summary>
        /// Gets SCC-related information about this environment.
        /// </summary>
        public string FolderSccInformation => SccProvider.FolderContentTitle;

        /// <summary>
        /// Gets or sets additional custom information about this environment.
        /// </summary>
        public string FolderCustomInformation { get; set; }

        /// <summary>
        /// Gets the enumeration of the custom commands.
        /// </summary>
        public IEnumerable<EnvironmentCustomCommand> CustomCommands => m_customCommands.Values; 

        public string EnvironmentInfo => ToString();

        // The properties in this region may not belong in this model-type, but it's the easy way.
        // TODO: Refactor to another mechanism conveying the view-only information to/from the configuration.
        #region Properties for saving view-only related-information.

        /// <summary>
        /// Gets or sets whether this environment item is viewable.
        /// For passing view information to the configuration.
        /// </summary>
        public bool IsViewable { get; set; }

        #endregion

        public override string ToString()
        {
            var infoList = new List<string>
            {
                FolderName,
                FolderSccInformationPresentable,
            };

            if (!string.IsNullOrEmpty(FolderCustomInformation))
            {
                infoList.Add(string.Format("[{0}]", FolderCustomInformation));
            }

            return string.Join(" | ", infoList);
        }

        /// <summary>
        /// Clears the custom commands collection and populates it with a configuration collection info.
        /// </summary>
        /// <param name="availableCommands">The collection of custom commands that can be used.</param>
        /// <param name="commandIds">Comman separated list of command IDs to be used for this environment.</param>
        public void PopulateCustomCommandsFromConfig(IList<EnvironmentCustomCommand> availableCommands, string commandIds)
        {
            if (string.IsNullOrEmpty(commandIds))
            {
                return;
            }

            s_logger.Info($"Using the following Custom Command IDs for environment in '{FolderName}': {commandIds}");
            var idsArray = commandIds.Split(SccEnvironmentConfigElement.CommandIdSeparator);

            m_customCommands.Clear();

            if (availableCommands == null)
            {
                return;
            }

            foreach (var command in availableCommands)
            {
                if (idsArray.Contains(command.Name))
                {
                    m_customCommands.Add(command.Name, command);
                }
            }
        }
    }
}
