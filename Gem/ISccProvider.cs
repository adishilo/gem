namespace Gem
{
    /// <summary>
    /// An SCC provider for the functionalities needed by this assembly
    /// </summary>
    public interface ISccProvider
    {
        /// <summary>
        /// Gets the SCC provider's name (technology).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the local folder's name.
        /// </summary>
        string LocalFolderName { get; }

        /// <summary>
        /// Gets the updated title to the contents of the configured folder, as a human readable string.
        /// </summary>
        string FolderContentTitlePresentable { get; }

        /// <summary>
        /// Gets the updated title to the contents of the configured folder.
        /// </summary>
        string FolderContentTitle { get; }

        /// <summary>
        /// Gets whether a given folder is currently related to an SCC.
        /// </summary>
        bool IsFolderConnectedToScc { get; }

        /// <summary>
        /// Gets the environment's info type.
        /// </summary>
        string InfoTypeDescription { get; }
    }
}
