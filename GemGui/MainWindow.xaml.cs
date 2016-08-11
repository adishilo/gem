using System.Windows;
using Gem.Configuration;
using GemGui.Commands;
using GemGui.ViewModel;

namespace GemGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var config = GemConfigManager.ConfigurationInfo;

            ViewModel = new GemViewModel(config.SearchRootFolder, config.MaxSearchDepth, this);

            ExitApplicationCommand = new ExitCommand(this);
            RefreshCommand = new RefreshStructureCommand(ViewModel);
            ConfigureAppCommand = new ConfigureCommand(ViewModel);
            LogViewingCommand = new ShowLogCommand();
            ConfigViewCommand = new ShowConfigurationCommand();
            AboutWindowCommand = new AboutCommand();

            InitializeComponent();

            OnlyHide = true;
        }

        #region Gui View Properties

        /// <summary>
        /// Gets the view model for the main window.
        /// </summary>
        public GemViewModel ViewModel { get; private set; }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the 'Exit' command for this window.
        /// </summary>
        public ExitCommand ExitApplicationCommand { get; }

        /// <summary>
        /// Gets the 'Refresh Folders Structure' command for this window.
        /// </summary>
        public RefreshStructureCommand RefreshCommand { get; }

        /// <summary>
        /// Gets the 'Configure' command for this window.
        /// </summary>
        public ConfigureCommand ConfigureAppCommand { get; }

        /// <summary>
        /// Gets the 'Show Log' command for this window.
        /// </summary>
        public ShowLogCommand LogViewingCommand { get; }

        /// <summary>
        /// Gets the 'Show Configuration' command for this window.
        /// </summary>
        public ShowConfigurationCommand ConfigViewCommand { get; }

        /// <summary>
        /// Gets the 'About window' command.
        /// </summary>
        public AboutCommand AboutWindowCommand { get; }

        #endregion

        /// <summary>
        /// Gets or sets whether to only hide this window.
        /// </summary>
        internal bool OnlyHide { get; set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = OnlyHide;

            if (OnlyHide)
            {
                Hide();
            }
            else
            {
                // Close additional dialogs that may be open:
                e.Cancel = !ConfigureAppCommand.CloseDialog();
                if (!e.Cancel)
                {
                    AboutWindowCommand.Close();
                }
            }
        }

        private void WndMain_Activated(object sender, System.EventArgs e)
        {
            Hide();
        }
    }
}
