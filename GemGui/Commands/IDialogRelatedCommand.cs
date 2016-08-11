namespace GemGui.Commands
{
    /// <summary>
    /// Defines commands that open dialogs.
    /// </summary>
    public interface IDialogRelatedCommand
    {
        /// <summary>
        /// Gets whether the dialog is open now.
        /// </summary>
        bool IsDialogOpen { get; }

        /// <summary>
        /// Close the dialog.
        /// </summary>
        /// <returns>Whether the dialog is closed at the end of this operation.</returns>
        bool CloseDialog();
    }
}
