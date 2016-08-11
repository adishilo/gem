using System;
using System.IO;
using Gem;
using Microsoft.Win32;

namespace HelperGui
{
    public class FilesBrowser : IEntitiesBrowser
    {
        #region IEntitiesBrowser implementation

        public virtual string Browse(string initialPath)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                InitialDirectory = GetInitialFolder(initialPath)
            };

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }

            return initialPath;
        }

        /// <summary>
        /// Process the given initial path to a relevant folder name.
        /// </summary>
        /// <param name="initialPath"></param>
        /// <returns></returns>
        protected virtual string GetInitialFolder(string initialPath)
        {
            if (string.IsNullOrEmpty(initialPath))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            if (Directory.Exists(initialPath))
            {
                return initialPath;
            }

            return Path.GetDirectoryName(Utils.StripPath(initialPath));
        }

        public virtual bool IsValidValue(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string strippedPath = Utils.StripPath(path);

            return File.Exists(strippedPath);
        }

        #endregion
    }
}