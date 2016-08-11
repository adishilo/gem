using System;
using System.IO;
using System.Windows.Forms;
using Gem;

namespace HelperGui
{
    public class FoldersBrowser : IEntitiesBrowser
    {
        #region IEntitiesBrowser implementation

        public string Browse(string initialPath)
        {
            initialPath = initialPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var chooseFolderDialog = new FolderBrowserDialog { SelectedPath = initialPath };

            if (chooseFolderDialog.ShowDialog() == DialogResult.OK)
            {
                return chooseFolderDialog.SelectedPath;
            }

            return initialPath;
        }

        public bool IsValidValue(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            string strippedPath = Utils.StripPath(path);

            return Directory.Exists(strippedPath);
        }

        #endregion
    }
}