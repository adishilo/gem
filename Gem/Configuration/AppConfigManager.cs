// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigManager.cs" company="" />
// <summary>
//   Manages configuration for an application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using NLog;
using SysConfig = System.Configuration;

namespace Gem.Configuration
{
    /// <summary>
    /// Manages configuration for an application.
    /// </summary>
    /// <typeparam name="TConfig">The type of the configuration section managed by this manager.</typeparam>
    public class AppConfigManager<TConfig>
        where TConfig : SysConfig.ConfigurationSection
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The sync object allowing only one configuration save at a time.
        /// </summary>
        private static readonly object s_saveConfigurationSync = new object();

        /// <summary>
        /// The timeout to allow a configuration save before waiving it with an error.
        /// </summary>
        private static readonly TimeSpan s_configurationSaveTimeout = TimeSpan.FromSeconds(30);

        private SysConfig.Configuration m_applicationConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigManager{TConfig}"/> class and creates the configuration section representation.
        /// The configuration is assumed to be on a file which is not the default exe.config file.
        /// </summary>
        /// <param name="configFileName">The configuration file name and path.</param>
        /// <param name="sectionName">The string representation of the configuration section in the configuration file.</param>
        public AppConfigManager(string configFileName, string sectionName)
        {
            OpenConfigurationFile(configFileName);

            var configuration = m_applicationConfiguration.GetSection(sectionName) as TConfig;

            if (configuration == null)
            {
                throw new SysConfig.ConfigurationErrorsException(
                    String.Format(CultureInfo.InvariantCulture, "Wrong configuration section type. Expected: {0}", typeof(TConfig).FullName));
            }

            RootConfigurationSection = configuration;
        }

        /// <summary>
        /// Gets the root configuration section representation.
        /// </summary>
        public TConfig RootConfigurationSection { get; private set; }

        /// <summary>
        /// Save the current application configuration.
        /// If cannot save within a timeout - issues an error and leaves without saving.
        /// </summary>
        public void SaveConfiguration()
        {
            try
            {
                lock (s_saveConfigurationSync)
                {
                    s_logger.Info("Start saving configuration and state.");

                    Task.Factory.StartNew(
                        () => m_applicationConfiguration
                            .Save(SysConfig.ConfigurationSaveMode.Minimal, false))
                            .Wait(s_configurationSaveTimeout);

                    s_logger.Info("Successfully saved configuration and state.");
                }
            }
            catch (AggregateException taskException)
            {
                taskException.Handle(ex =>
                {
                    string errorMessage;

                    if (m_applicationConfiguration == null)
                    {
                        errorMessage = String.Format(
                            CultureInfo.InvariantCulture,
                            "Could not save configuration object within {0}. Exception: {1}",
                            s_configurationSaveTimeout,
                            ex);

                        s_logger.Error(errorMessage);
                    }
                    else
                    {
                        errorMessage = String.Format(
                            CultureInfo.InvariantCulture,
                            "Could not save configuration object to {0} within {1}. Exception: {2}",
                            m_applicationConfiguration.FilePath,
                            s_configurationSaveTimeout,
                            ex);

                        s_logger.Error(errorMessage);
                    }

                    return false;
                });
            }
        }

        /// <summary>
        /// Open the configuration file for the GEM application.
        /// If it does not exist, creates it out of the default exe.config file.
        /// </summary>
        /// <param name="configFileName">configFileName</param>
        private void OpenConfigurationFile(string configFileName)
        {
            s_logger.Info("Opening configuration file {0}", configFileName);

            if (!File.Exists(configFileName))
            {
                s_logger.Info("Configuration file does not exist, copying the default exe.config");

                // First create the configuration file:
                File.Copy(AppDomain.CurrentDomain.FriendlyName + ".config", configFileName);
            }

            var fileMap = new SysConfig.ConfigurationFileMap(configFileName);
            m_applicationConfiguration = SysConfig.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
        }
    }
}
