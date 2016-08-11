using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Gem;
using GemGui.ViewModel;

namespace GemGui
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    public partial class ConfigureWindow : Window
    {
        public ConfigureWindow()
            : this(null)
        {
        }

        public ConfigureWindow(ConfigurationViewModel viewModel)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ViewModel = viewModel ??
                new ConfigurationViewModel(
                    Enumerable.Empty<EnvironmentCustomCommand>(),
                    Enumerable.Empty<EnvironmentViewModel>())
                { MaxSearchDepth = 1 };

            InitializeComponent();
        }

        #region Visual Properties

        /// <summary>
        /// Gets the view model for this dialog.
        /// </summary>
        public ConfigurationViewModel ViewModel { get; }

        #endregion

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
