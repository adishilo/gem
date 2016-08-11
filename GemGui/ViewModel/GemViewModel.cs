using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Gem;
using Gem.Configuration;
using GemGui.Annotations;
using NLog;

namespace GemGui.ViewModel
{
    /// <summary>
    /// The View Model for the Gem GUI application.
    /// </summary>
    public class GemViewModel : INotifyPropertyChanged
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private static readonly ImageSource s_trayIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/GemAppIcon.ico"));
        private static readonly ImageSource s_trayEditingIcon = new BitmapImage(new Uri("pack://application:,,,/Icons/GemAppEditModeIcon.ico"));

        private readonly TimeSpan m_refreshSpan = TimeSpan.FromSeconds(5);

        private readonly Timer m_refreshInfoTimer;
        private readonly GeDefinitionsManager m_sccManager = new GeDefinitionsManager();

        private string m_rootFolder = null;
        private int m_maxSearchDepth = 3;

        /// <summary>
        /// Create a new instance of <see cref="GemViewModel"/> initializing it without a root folder.
        /// </summary>
        /// <param name="ownerWindow">The owner window to this model view.</param>
        public GemViewModel(MainWindow ownerWindow)
            : this(null, 1, ownerWindow)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="GemViewModel"/> initializing it to monitor a given root folder.
        /// </summary>
        /// <param name="rootFolder">The root folder to monitor.</param>
        /// <param name="maxDepth">The maximum depth under the root folder to look for SCC bound folders.</param>
        /// <param name="ownerWindow">The owner window to this model view.</param>
        public GemViewModel(string rootFolder, int maxDepth, MainWindow ownerWindow)
        {
            MaxSearchDepth = maxDepth;
            RootFolder = rootFolder;
            OwnerWindow = ownerWindow;

            RefreshFoldersStructure(true);

            SccEnvironmentsModel = new SccEnvironmentsViewModel(m_sccManager);

            InitTrayIconHandling();

            m_refreshInfoTimer = new Timer(
                state =>
                {
                    OnPropertyChanged(nameof(SccSummary));
                    SccEnvironmentsModel.RefreshEnvironmentsInformationPresented();
                },
                null,
                TimeSpan.Zero,
                m_refreshSpan);
        }

        private void InitTrayIconHandling()
        {
            SetTrayIcon(false);

            SccEnvironmentsModel.OnEnvironmentEditStart += (sender, args) => SetTrayIcon(true);
            SccEnvironmentsModel.OnEnvironmentEditDone += (sender, args) => SetTrayIcon(false);
        }

        /// <summary>
        /// Gets or sets the root folder to scan for SCC presence.
        /// </summary>
        public string RootFolder
        {
            get
            {
                return m_rootFolder;
            }

            set
            {
                m_rootFolder = value;

                GemConfigManager.ConfigurationInfo.SearchRootFolder = m_rootFolder;
            }
        }

        /// <summary>
        /// Gets the maximum depth under the root folder to look for SCC bound folders.
        /// </summary>
        public int MaxSearchDepth
        {
            get
            {
                return m_maxSearchDepth;
            }

            set
            {
                if (value > 0)
                {
                    m_maxSearchDepth = value;

                    GemConfigManager.ConfigurationInfo.MaxSearchDepth = value;
                }
            }
        }

        /// <summary>
        /// Gets the available custom commands.
        /// </summary>
        public IEnumerable<EnvironmentCustomCommand> CustomCommands => m_sccManager.CustomCommands; 

        #region Visual properties

        /// <summary>
        /// Gets the owner window of this model view.
        /// </summary>
        public MainWindow OwnerWindow { get; private set; }

        /// <summary>
        /// Gets the SCC summary for the defined root folder.
        /// </summary>
        public string SccSummary
        {
            get
            {
                string envContents;

                SccEnvironmentsModel.RefreshEnvironmentsView();

                if (m_sccManager.IsPopulating)
                {
                    envContents = string.Format(
                        CultureInfo.InvariantCulture,
                        "Populating from root {0}, max depth {1}..",
                        RootFolder,
                        MaxSearchDepth);
                }
                else
                {
                    if (m_sccManager.EnvironmentsCount > 0)
                    {
                        envContents = string.Join(
                            Environment.NewLine,
                            m_sccManager.Environments.Where(env => env.IsViewable));
                    }
                    else
                    {
                        envContents = "No SCC related folders found."
                                      + Environment.NewLine
                                      + "Maybe configure a root folder for search?";
                    }
                }

                int pickedEnvCount = m_sccManager.Environments.Count(env => env.IsViewable);
                string pickedEnvStatus = pickedEnvCount == m_sccManager.EnvironmentsCount
                    ? "Environments"
                    : $"(Showing {pickedEnvCount}/{m_sccManager.EnvironmentsCount} Environments)";

                return $"GEM {pickedEnvStatus}{Environment.NewLine}---{Environment.NewLine}{envContents}";
            }
        }

        /// <summary>
        /// Gets the view model for the SCC environments list control <see cref="SccEnvironmentsControl"/>.
        /// </summary>
        public SccEnvironmentsViewModel SccEnvironmentsModel { get; private set; }

        /// <summary>
        /// Gets or sets the icon displayed on the systray.
        /// </summary>
        public ImageSource ApplicationTrayIcon { get; private set; }

        #endregion

        /// <summary>
        /// Repopulate the managed SCC environments according to the current structure.
        /// Assumes the existence of a value for <see cref="RootFolder"/> and <see cref="MaxSearchDepth"/> properties.
        /// Does nothing if these properties contain invalid values.
        /// </summary>
        public async Task RefreshFoldersStructure(bool consolidateFromConfiguration = false)
        {
            if (!m_sccManager.IsPopulating &&
                MaxSearchDepth > 0 &&
                !string.IsNullOrEmpty(RootFolder) &&
                Directory.Exists(RootFolder))
            {
                await Task.Factory.StartNew(() => m_sccManager.PopulateEnvironments(RootFolder, MaxSearchDepth));

                if (consolidateFromConfiguration)
                {
                    m_sccManager.ConsolidateConfiguration();
                }

                m_sccManager.SaveConfiguration();
            }
        }

        /// <summary>
        /// Sets the tray icon according to the editing mode.
        /// </summary>
        /// <param name="isEditing"></param>
        private void SetTrayIcon(bool isEditing)
        {
            ApplicationTrayIcon = isEditing
                ? s_trayEditingIcon
                : s_trayIcon;

            OnPropertyChanged(nameof(ApplicationTrayIcon));
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
