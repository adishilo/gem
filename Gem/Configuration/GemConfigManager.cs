using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using NLog;

namespace Gem.Configuration
{
    /// <summary>
    /// A configuration manager
    /// </summary>
    public static class GemConfigManager
    {
        private const string c_appNameInSystemDataFolder = "Gem";
        private const string c_relativeConfigurationFileName = "GemConfiguration.config";

        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private static readonly AppConfigManager<GemConfigSection> s_appConfigManager;

        static GemConfigManager()
        {
            try
            {
                OpenOrCreateConfigurationFile();

                s_appConfigManager = new AppConfigManager<GemConfigSection>(UserConfigFileName, GemConfigSection.SectionName);

                ConfigurationInfo = s_appConfigManager.RootConfigurationSection;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format(
                    "Gem could not load configuration, probably due to a corrupt configuration file." +
                    "{0}Either fix the corruption in file {1} or delete it." +
                    "{0}{0}Error: {2}",
                    Environment.NewLine,
                    GemConfigManager.UserConfigFileName,
                    ex.Message);

                s_logger.Error(ex, errorMessage);

                throw new ConfigurationErrorsException(errorMessage);
            }
        }

        public static string UserConfigFileName { get; private set; }

        /// <summary>
        /// Gets the configuration section of the application.
        /// </summary>
        public static GemConfigSection ConfigurationInfo { get; }

        /// <summary>
        /// Save a new set of configuration section.
        /// </summary>
        /// <param name="environments">Enumeration of the SCC environments available in memory.</param>
        /// <param name="customCommands">The available custom commands.</param>
        public static void SaveConfiguration(IEnumerable<EnvironmentDefinition> environments, IEnumerable<EnvironmentCustomCommand> customCommands)
        {
            UpdateEnvironmentsConfigData(environments);
            UpdateCustomCommandsConfigData(customCommands);

            s_appConfigManager.SaveConfiguration();
        }

        private static void UpdateEnvironmentsConfigData(IEnumerable<EnvironmentDefinition> environments)
        {
            ConfigurationInfo.SccEnvironments.Clear();
            foreach (var environment in environments)
            {
                var newEnvironmentConfig = new SccEnvironmentConfigElement
                {
                    Folder = environment.FolderName,
                    SccInfo = environment.FolderSccInformationPresentable,
                    CustomInfo = environment.FolderCustomInformation,
                    IsViewable = environment.IsViewable,
                    CustomCommandIds = string.Join(SccEnvironmentConfigElement.CommandIdSeparator.ToString(), environment.CustomCommands.Select(command => command.Name))
                };

                ConfigurationInfo.SccEnvironments.Add(newEnvironmentConfig);
            }
        }

        private static void UpdateCustomCommandsConfigData(IEnumerable<EnvironmentCustomCommand> customCommands)
        {
            ConfigurationInfo.CustomCommands.Clear();
            foreach (var command in customCommands)
            {
                var newCommandConfig = new CommandConfigElement()
                {
                    Name = command.Name,
                    ExecutablePath = command.ExecutableCommand,
                    Parameters = command.Parameters,
                    WorkingDirectory = command.WorkingDirectory,
                    RunElevated = command.RunElevated,
                    Description = command.Description
                };

                ConfigurationInfo.CustomCommands.Add(newCommandConfig);
            }
        }

        private static void OpenOrCreateConfigurationFile()
        {
            string userConfigFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), c_appNameInSystemDataFolder);

            // Make sure the directory exists:
            Directory.CreateDirectory(userConfigFolder);

            UserConfigFileName = Path.Combine(userConfigFolder, c_relativeConfigurationFileName);
        }
    }
}
