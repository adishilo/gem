using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Gem;
using GemGui.Annotations;
using GemGui.Commands;
using NLog;
using Brush = System.Windows.Media.Brush;

namespace GemGui.ViewModel
{
    /// <summary>
    /// A view model for a <see cref="EnvironmentDefinition"/> object.
    /// </summary>
    public class EnvironmentViewModel : INotifyPropertyChanged
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly EnvironmentDefinition m_modelDefinition;

        private bool m_showInfo;
        private string m_lastCustomInformation;

        public EnvironmentViewModel(EnvironmentDefinition modelDefinition, SccEnvironmentsViewModel container, int index)
        {
            Utils.GuardNotNull(modelDefinition, "relatedDefinition");

            m_modelDefinition = modelDefinition;
            Container = container;
            ShowInfo = true;

            NonHoverColor = index % 2 == 0
                ? new SolidColorBrush(Colors.White)
                : new SolidColorBrush(Colors.PapayaWhip);

            SetItemContextMenu();
        }

        /// <summary>
        /// Generate and set the context-menu for this item.
        /// The context menu for this environment-view-model is cached, because the whole environments view model
        /// is build every now and then to accomodate changes in the status of the underlying represented SCC environment,
        /// and there's no need in building that menu every so often.
        /// </summary>
        private void SetItemContextMenu()
        {
            AddStaticContextMenu();

            AddCustomContextMenu();
        }

        private void AddCustomContextMenu()
        {
            if (m_modelDefinition.CustomCommands.Any())
            {
                s_logger.Info($"Creating menu-items for environment: {m_modelDefinition.FolderName}..");
                AddCustomMenuBanner();

                foreach (var command in m_modelDefinition.CustomCommands)
                {
                    string calculatedParameters = command.Parameters;
                    string calculatedWorkingDir = command.WorkingDirectory;
                    string toolTipCommand = $"Run: {Utils.GetFileName(command.ExecutableCommand)} {calculatedParameters}";
                    string toolTipFolder = string.IsNullOrEmpty(calculatedWorkingDir)
                        ? string.Empty
                        : $"{Environment.NewLine}In: {Utils.StripPath(calculatedWorkingDir)}";
                    string elevatedText = command.RunElevated ? "(Elevated) " : string.Empty;

                    GeneratedContextMenu.AddMenuItem(
                        command.Name,
                        Container.ExecutableRunCommand,
                        new ProcessExecutionDefinition(
                            command.ExecutableCommand,
                            new LazyEvalString(calculatedParameters, CommandFormat),
                            new LazyEvalString(calculatedWorkingDir, CommandFormat),
                            command.RunElevated),
                        null,
                        new LazyEvalString($"{elevatedText}{command.Description}{Environment.NewLine}{toolTipCommand}{toolTipFolder}", CommandFormat));

                    s_logger.Info(
                        CommandFormat($"Menu-item {command.Name}: {command.ExecutableCommand} {calculatedParameters} Working directory: {calculatedWorkingDir}"));
                }
            }
        }

        /// <summary>
        /// A formatter for the lazy evaluation of the format strings comprising the parameters, workspace and tool-tips.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CommandFormat(string input)
        {
            var formatter = new StringCustomReplacer();

            formatter.RegisterKeyword("FolderName", FolderName);
            formatter.RegisterKeyword("CustomInfo", FolderSccInformation);

            return formatter.Format(input);
        }

        private void AddCustomMenuBanner()
        {
            GeneratedContextMenu.Items.Add(new Separator());
            GeneratedContextMenu.Items.Add(
                new MenuItem
                {
                    Header = "Custom Operations",
                    Background = Brushes.Blue,
                    Foreground = Brushes.White,
                    FontSize = 9,
                    IsHitTestVisible = false,
                    StaysOpenOnClick = true
                });
        }

        private void AddStaticContextMenu()
        {
            GeneratedContextMenu = new ContextMenu();

            GeneratedContextMenu.AddMenuItem(
                "Explore Here", Container.ExecutableRunCommand, new ProcessExecutionDefinition("explorer.exe", $"\"{FolderName}\""), "Icons/Folder.ico");
            GeneratedContextMenu.Items.Add(new Separator());
            GeneratedContextMenu.AddMenuItem("Copy Path", Container.CopyTextCommand, $"\"{FolderName}\"", "Icons/CopyPath.ico");
            GeneratedContextMenu.AddMenuItem("Copy Info", Container.CopyTextCommand, FolderSccInformationPresentable, "Icons/CopyInfo.ico");
        }

        #region Visual properties

        /// <summary>
        /// Gets the parent container to this environment view model.
        /// </summary>
        public SccEnvironmentsViewModel Container { get; }

        public ContextMenu GeneratedContextMenu { get; private set; }

        /// <summary>
        /// Gets the folder name.
        /// </summary>
        public string FolderName => m_modelDefinition.FolderName;

        /// <summary>
        /// Gets the SCC related information for the environment's folder, in human readble fashion.
        /// </summary>
        public string FolderSccInformationPresentable => m_modelDefinition.FolderSccInformationPresentable;

        /// <summary>
        /// Gets the SCC related information for the environment's folder.
        /// </summary>
        public string FolderSccInformation => m_modelDefinition.FolderSccInformation;

        /// <summary>
        /// Gets or sets user custom information used to describe the SCC-related folder.
        /// </summary>
        public string FolderCustomInformation
        {
            get
            {
                return m_modelDefinition.FolderCustomInformation;
            }

            set
            {
                m_modelDefinition.FolderCustomInformation = value;
            }
        }

        /// <summary>
        /// Gets or sets whether this environment can be viewed on the quick-view.
        /// </summary>
        public bool IsOnQuickView
        {
            get
            {
                return m_modelDefinition.IsViewable;
            }

            set
            {
                m_modelDefinition.IsViewable = value;

                Container.OnPropertyChanged("EnvironmentsStatus");
                CommitEnvironmentInformationTransaction();
            }
        }

        /// <summary>
        /// Gets the information to present for an SCC environment.
        /// </summary>
        public string Info
        {
            get
            {
                string customInfo = string.Empty;

                if (!string.IsNullOrEmpty(FolderCustomInformation) && ShowInfo)
                {
                    customInfo = $"{Environment.NewLine}{FolderCustomInformation}";
                }

                return $"{m_modelDefinition.FolderName} | {m_modelDefinition.FolderSccInformationPresentable}{customInfo}";
            }
        }

        /// <summary>
        /// Gets whether to show the environment's info.
        /// </summary>
        public bool ShowInfo
        {
            get
            {
                return m_showInfo;
            }

            set
            {
                m_showInfo = value;

                OnPropertyChanged(nameof(Info));
                OnPropertyChanged(nameof(IsCustomInfoVisible));
            }
        }

        /// <summary>
        /// Gets whether to show the custom information text-box (for property <see cref="FolderCustomInformation"/>.
        /// Is the opposite of <see cref="IsInfoVisible"/>.
        /// </summary>
        public Visibility IsCustomInfoVisible => ShowInfo ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// The brush defining the visible background of the environment view when not hovering over with the mouse.
        /// </summary>
        public Brush NonHoverColor { get; private set; }

        #endregion

        #region Actions

        /// <summary>
        /// Set/reset the control edit mode.
        /// </summary>
        /// <param name="canEdit">Whether the control allows editing.</param>
        public void SetEditable(bool canEdit)
        {
            ShowInfo = !canEdit;

            Container.EnableRefreshEnvironment = !canEdit; // Stop refreshing for the editing while
        }

        public void StartCustomInformationTransaction()
        {
            m_lastCustomInformation = FolderCustomInformation;
        }

        public void RollbackCustomInformationTransaction()
        {
            FolderCustomInformation = m_lastCustomInformation;

            OnPropertyChanged(nameof(FolderCustomInformation));
            OnPropertyChanged(nameof(Info));
        }

        public void CommitEnvironmentInformationTransaction()
        {
            OnPropertyChanged(nameof(Info));

            Container.SaveConfiguration();
        }

        internal void RefreshInformationPresented()
        {
            OnPropertyChanged(nameof(ShowInfo));
            OnPropertyChanged(nameof(Info));
            OnPropertyChanged(nameof(IsCustomInfoVisible));
        }

        #endregion

        #region INotifyProperyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
