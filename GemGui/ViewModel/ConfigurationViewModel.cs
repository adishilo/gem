using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Gem;
using GemGui.Annotations;
using Microsoft.Win32;
using NLog;

namespace GemGui.ViewModel
{
    /// <summary>
    /// Serves as a view model for the configuration dialog, and for passing that information.
    /// </summary>
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The application name as it appears in the value in the registry for 'Start with Windows' feature.
        /// Never change this value.
        /// </summary>
        private const string c_registryAppName = "Gem";

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private int m_maxSearchDepth;
        private string m_rootFolderForSearch;
        private bool m_startWithWindows;

        public ConfigurationViewModel(IEnumerable<EnvironmentCustomCommand> customCommands, IEnumerable<EnvironmentViewModel> environmentsView)
        {
            CustomCommands = new ObservableCollection<CustomCommandViewModel>(
                customCommands.Select(command => new CustomCommandViewModel(command)));

            environmentsView = environmentsView ?? Enumerable.Empty<EnvironmentViewModel>();
            EnvironmentsView = new ObservableCollection<ConfigEnvironmentViewModel>(environmentsView.Select(env => new ConfigEnvironmentViewModel(env, false)));

            OnPropertyChanged(nameof(RootFolderForSearch));
        }

        /// <summary>
        /// Gets or sets the configured root folder for search.
        /// </summary>
        public string RootFolderForSearch
        {
            get
            {
                return m_rootFolderForSearch;
            }

            set
            {
                m_rootFolderForSearch = value;
           
                OnPropertyChanged(nameof(RootFolderForSearch));
            }
        }

        /// <summary>
        /// Gets or sets the maximum folder depth for SCC search.
        /// </summary>
        public int MaxSearchDepth
        {
            get
            {
                return m_maxSearchDepth;
            }

            set
            {
                m_maxSearchDepth = value;
           
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the available custom commands currently configured.
        /// </summary>
        public ObservableCollection<CustomCommandViewModel> CustomCommands { get; }

        /// <summary>
        /// Gets the view modesl of the currently available environments.
        /// </summary>
        public ObservableCollection<ConfigEnvironmentViewModel> EnvironmentsView { get; }

        /// <summary>
        /// Gets or sets whether the application is registered to start with Windows in the current user's profile.
        /// </summary>
        public bool StartWithWindows
        {
            get
            {
                using (var regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    m_startWithWindows = regKey.GetValue(c_registryAppName) != null;

                    return m_startWithWindows;
                }
            }

            set
            {
                using (var regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (m_startWithWindows)
                    {
                        // Removing the option:
                        s_logger.Info("Removing the option to start with Windows.");

                        try
                        {
                            regKey.DeleteValue(c_registryAppName);
                        }
                        catch (ArgumentException argEx)
                        {
                            s_logger.Error(argEx, $"The registry value of '{c_registryAppName}' was not found.");

                            m_startWithWindows = false;
                        }
                        catch (Exception ex)
                        {
                            s_logger.Error(ex, "Could not remove the registry key.");

                            MessageBox.Show("Failed to remove the option to start with Windows.", "GEM", MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        // Setting the option:
                        s_logger.Info("Setting the option to start with Windows.");

                        try
                        {
                            regKey.SetValue(c_registryAppName, Environment.CommandLine, RegistryValueKind.String);
                        }
                        catch (Exception ex)
                        {
                            s_logger.Error(ex, "Could not set the registry key.");

                            MessageBox.Show("Failed to set the options to start with Windows.", "GEM", MessageBoxButton.OK);
                        }
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
