using System.ComponentModel;
using HelperGui.Converters;

namespace HelperGui
{
    /// <summary>
    /// Defines how to browse the entities it represents.
    /// </summary>
    [TypeConverter(typeof(StringToIEntitiesBrowser))]
    public interface IEntitiesBrowser
    {
        /// <summary>
        /// Return the choice of path, and allow an intial path to be given.
        /// </summary>
        /// <param name="initialPath">The path to present at first, and start with.</param>
        /// <returns>The final chosen path.</returns>
        string Browse(string initialPath);

        /// <summary>
        /// Gets whether the given path is valid according to the definition of the entities browser.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns>Whether the given path is valid.</returns>
        bool IsValidValue(string path);
    }
}