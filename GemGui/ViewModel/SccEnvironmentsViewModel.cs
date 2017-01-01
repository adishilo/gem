using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Gem;
using GemGui.Annotations;
using GemGui.Commands;

namespace GemGui.ViewModel
{
    /// <summary>
    /// View model for the <see cref="SccEnvironmentsControl"/>.
    /// </summary>
    public class SccEnvironmentsViewModel : INotifyPropertyChanged
    {
        private readonly GeDefinitionsManager m_sccManager;

        private EnvironmentViewModel m_currentEditingView = null;
        private ObservableCollection<EnvironmentViewModel> m_currentEnvironmentsViewModel = null;
        private bool m_environmentsDirty = false;

        public SccEnvironmentsViewModel(GeDefinitionsManager sccManager)
        {
            m_sccManager = sccManager;
            m_sccManager.OnDonePopulating +=
                (sender, args) =>
                {
                    SetEnvironmentsDirty();
                };

            CopyTextCommand = new CopyCommand();
            ExecutableRunCommand = new ParameterizedProcessExecutorCommand();
            EditCustomItemCommand = new EditCustomFolderInfoCommand(this);

            EnableRefreshEnvironment = true;
        }

        /// <summary>
        /// Gets or sets whether the refreshing the environments view takes place.
        /// </summary>
        public bool EnableRefreshEnvironment { get; set; }

        #region Visual properties

        /// <summary>
        /// Gets the items representing the environments' view.
        /// </summary>
        public ObservableCollection<EnvironmentViewModel> EnvironmentsView
        {
            get
            {
                if (m_sccManager.IsPopulating)
                {
                    // Currently populating environments:
                    m_currentEnvironmentsViewModel = new ObservableCollection<EnvironmentViewModel>();
                }
                else
                    if (m_environmentsDirty)
                    {
                        // There's a new update to the environments:
                        int index = 0;

                        m_currentEnvironmentsViewModel =
                            new ObservableCollection<EnvironmentViewModel>(
                                m_sccManager.Environments.Select(environment => new EnvironmentViewModel(environment, this, index++)));

                        m_environmentsDirty = false;
                    }

                return m_currentEnvironmentsViewModel;
            }
        }

        /// <summary>
        /// Gets whether there's any environments information to present.
        /// </summary>
        public bool HasEnvironments
        {
            get
            {
                return m_sccManager.EnvironmentsCount > 0;
            }
        }

        public string EnvironmentsStatus
        {
            get
            {
                if (m_sccManager.IsPopulating)
                {
                    return "Populating..";
                }

                if (!HasEnvironments)
                {
                    return "Nothing identified";
                }

                int countViewableEnvironments = m_sccManager.Environments.Count(env => env.IsViewable);

                return countViewableEnvironments == m_sccManager.EnvironmentsCount
                    ? $"{m_sccManager.EnvironmentsCount} Environments"
                    : $"{countViewableEnvironments}/{m_sccManager.EnvironmentsCount} Environments viewable";
            }
        }

        #endregion

        #region Events

        public event EventHandler OnEnvironmentEditStart;
        public event EventHandler OnEnvironmentEditDone;

        #endregion

        #region Commands

        /// <summary>
        /// The command to copy a specific text to the clipboard.
        /// </summary>
        public CopyCommand CopyTextCommand { get; }

        /// <summary>
        /// The command to run predefined executables.
        /// </summary>
        public ParameterizedProcessExecutorCommand ExecutableRunCommand { get; }

        /// <summary>
        /// The command to handle clicking a folder-info text item for editing the custom info.
        /// </summary>
        public EditCustomFolderInfoCommand EditCustomItemCommand { get; }

        #endregion

        /// <summary>
        /// Sets or resets a specific environment view model's edit mode.
        /// </summary>
        /// <param name="environmentToSet">The environment view model to set/reset for editing.</param>
        /// <param name="editMode">The edit mode to apply.</param>
        /// <param name="customTextBox">The editing text-box.</param>
        public void SetEditable(EnvironmentViewModel environmentToSet, EditCommandModes editMode, TextBox customTextBox)
        {
            switch (editMode)
            {
                case EditCommandModes.InvokeEditMode:
                {
                    if (m_currentEditingView != null)
                    {
                        m_currentEditingView.SetEditable(false);
                    }

                    environmentToSet.StartCustomInformationTransaction();
                    environmentToSet.SetEditable(true);
                    m_currentEditingView = environmentToSet;
                    
                    customTextBox.Focus();
                    customTextBox.SelectAll();

                    OnEnvironmentEditStart(this, EventArgs.Empty);

                    break;
                }

                case EditCommandModes.CancelEditMode:
                {
                    environmentToSet.SetEditable(false);
                    m_currentEditingView = null;
                    environmentToSet.RollbackCustomInformationTransaction();

                    OnEnvironmentEditDone(this, EventArgs.Empty);

                    break;
                }

                case EditCommandModes.ApplyEdit:
                {
                    environmentToSet.SetEditable(false);
                    m_currentEditingView = null;
                    environmentToSet.CommitEnvironmentInformationTransaction();

                    OnEnvironmentEditDone(this, EventArgs.Empty);

                    break;
                }

                default:
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unrecognized enum {0}", editMode));
                }
            }
        }

        /// <summary>
        /// Update the view of the SCC environments, may cause re-instantiation of all environments.
        /// </summary>
        public void RefreshEnvironmentsView()
        {
            if (EnableRefreshEnvironment)
            {
                OnPropertyChanged(nameof(EnvironmentsView));
                OnPropertyChanged(nameof(HasEnvironments));
                OnPropertyChanged(nameof(EnvironmentsStatus));
            }
        }

        /// <summary>
        /// Save the current state of the environments into the configuration.
        /// </summary>
        public void SaveConfiguration()
        {
            m_sccManager.SaveConfiguration();
        }

        /// <summary>
        /// Make the environments information update.
        /// </summary>
        internal void SetEnvironmentsDirty()
        {
            m_environmentsDirty = true;
        }

        /// <summary>
        /// Used to refresh just the contents of the available environments.
        /// </summary>
        internal void RefreshEnvironmentsInformationPresented()
        {
            if (m_currentEnvironmentsViewModel != null)
            {
                foreach (var environmentViewModel in m_currentEnvironmentsViewModel)
                {
                    environmentViewModel.RefreshInformationPresented();
                }
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
