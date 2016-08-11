namespace GemGui.ViewModel
{
    /// <summary>
    /// View model container for information regarding the environment presented in the configuration dialog.
    /// </summary>
    public class ConfigEnvironmentViewModel
    {
        public ConfigEnvironmentViewModel(EnvironmentViewModel envViewModel, bool isSelected)
        {
            EnvViewModel = envViewModel;
            IsSelected = isSelected;
        }

        /// <summary>
        /// Gets the related environment view model.
        /// </summary>
        public EnvironmentViewModel EnvViewModel { get; }

        /// <summary>
        /// Gets whether this environment is selected.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}