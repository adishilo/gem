using System.Windows.Controls;
using GemGui.ViewModel;

namespace GemGui.Commands
{
    /// <summary>
    /// Defines the parameters to pass for the <see cref="EditCustomFolderInfoCommand"/> command.
    /// </summary>
    public class EnvironmentEditingDefinition
    {
        public EnvironmentEditingDefinition(EditCommandModes commandMode, EnvironmentViewModel environmentModel, TextBox editingTextBox)
        {
            CommandMode = commandMode;
            EnvironmentModel = environmentModel;
            EditingTextBox = editingTextBox;
        }

        public EditCommandModes CommandMode { get; private set; }
        public EnvironmentViewModel EnvironmentModel { get; private set; }
        public TextBox EditingTextBox { get; private set; }
    }
}
