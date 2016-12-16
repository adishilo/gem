using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Gem.Configuration;
using NLog;

namespace Gem
{
    /// <summary>
    /// Manages the environmnet definitions.
    /// </summary>
    public class GeDefinitionsManager
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Maps the workspace folder to the environment's definition.
        /// </summary>
        private readonly Dictionary<string, EnvironmentDefinition> m_environments;

        /// <summary>
        /// The list of available configured custom commands.
        /// </summary>
        private readonly List<EnvironmentCustomCommand> m_customCommands =
            GemConfigManager.ConfigurationInfo.CustomCommands
                .Cast<CommandConfigElement>()
                .Select(configCommand => new EnvironmentCustomCommand(configCommand)).ToList();


        /// <summary>
        /// Creates a new instance of <see cref="GeDefinitionsManager"/>.
        /// </summary>
        public GeDefinitionsManager()
        {
            m_environments = new Dictionary<string, EnvironmentDefinition>();

            IsPopulating = false;
        }

        /// <summary>
        /// Triggered when the environments manager is done populating environments.
        /// </summary>
        public event EventHandler OnDonePopulating;

        /// <summary>
        /// Gets the enumeration of the managed environments.
        /// </summary>
        public IEnumerable<EnvironmentDefinition> Environments
        {
            get
            {
                lock (m_environments)
                {
                    return m_environments.Values.ToList();
                }
            }
        }

        /// <summary>
        /// Gets the count of environments identified and available.
        /// </summary>
        public int EnvironmentsCount
        {
            get
            {
                lock (m_environments)
                {
                    return m_environments.Count;
                }
            }
        }

        /// <summary>
        /// Gets whether the environments' information is now being populated (for refreshing folder structure).
        /// </summary>
        public bool IsPopulating { get; private set; }

        /// <summary>
        /// Gets the currently available custom commands.
        /// </summary>
        public IEnumerable<EnvironmentCustomCommand> CustomCommands => m_customCommands;

        /// <summary>
        /// Remove a specific environment from the managed collection.
        /// </summary>
        /// <param name="folderName"></param>
        public void RemoveEnvironment(string folderName)
        {
            lock (m_environments)
            {
                if (m_environments.ContainsKey(folderName))
                {
                    m_environments.Remove(folderName);
                }
            }
        }

        /// <summary>
        /// Recursively searches for folders hosting SCC and add them to the collection of environments.
        /// This operation clears the environments' collection and starts over.
        /// This operation is NOT thread-safe.
        /// </summary>
        /// <param name="rootFolder">The root folder where to start searching.</param>
        /// <param name="maxDepth">The maximum depth to search.</param>
        public void PopulateEnvironments(string rootFolder, int maxDepth)
        {
            lock (m_environments)
            {
                if (!Directory.Exists(rootFolder))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Folder '{0}' does not exist.", rootFolder));
                }

                IsPopulating = true;

                m_environments.Clear();

                PopulateEnvironmentEx(rootFolder, maxDepth, 0);
            
                IsPopulating = false;

                OnDonePopulating?.Invoke(this, new EventArgs());
            }
        }

        private void PopulateEnvironmentEx(string rootFolder, int maxDepth, int depth)
        {
            foreach (var folder in Directory.EnumerateDirectories(rootFolder))
            {
                var sccProvider = SccProvidersStaticUtils.Instance.GetProvider(folder);

                if (sccProvider != null)
                {
                    m_environments.Add(folder, new EnvironmentDefinition(sccProvider));
                }
                else
                {
                    if (depth < maxDepth)
                    {
                        PopulateEnvironmentEx(folder, maxDepth, depth + 1);
                    }
                }
            }
        }

        public void ConsolidateConfiguration()
        {
            var environmentsConfig = GemConfigManager.ConfigurationInfo.SccEnvironments;

            // Pass information from the environments in the configuration to the environments identified by the
            // populate process, identified by the folder and SCC information set to them.
            // Any environment in the configuration not in the populated set - will be eliminated upon save-configuration.
            // Any environment in the populated set not in the configuration - will be new to the configuration.
            foreach (var environmentConf in environmentsConfig.Cast<SccEnvironmentConfigElement>())
            {
                var matchEnvironmentsQuery =
                    from env in Environments
                    where
                        env.FolderName.Equals(environmentConf.Folder)
                    select env;

                foreach (var matchedEnvironment in matchEnvironmentsQuery)
                {
                    matchedEnvironment.FolderCustomInformation = environmentConf.CustomInfo;
                    matchedEnvironment.IsViewable = environmentConf.IsViewable;
                    matchedEnvironment.PopulateCustomCommandsFromConfig(m_customCommands, environmentConf.CustomCommandIds);
                }
            }

            s_logger.Info("Configuration consolidated.");
        }

        /// <summary>
        /// Save the current state of the environments into the configuration.
        /// </summary>
        public void SaveConfiguration()
        {
            GemConfigManager.SaveConfiguration(Environments, m_customCommands);
        }
    }
}
