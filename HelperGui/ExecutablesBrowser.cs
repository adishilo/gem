using System;
using System.IO;
using System.Linq;
using Gem;
using Microsoft.Win32;

namespace HelperGui
{
    public class ExecutablesBrowser : FilesBrowser
    {
        private const string c_executableExtension = ".exe";

        public override string Browse(string initialPath)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = $"Executable files (*{c_executableExtension})|*{c_executableExtension}|All files (*.*)|*.*",
                InitialDirectory = GetInitialFolder(initialPath)
            };

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }

            return initialPath;
        }

        protected override string GetInitialFolder(string initialPath)
        {
            initialPath = initialPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string resultPath = base.GetInitialFolder(initialPath);
            string executableName = Path.GetFileNameWithoutExtension(Utils.StripPath(initialPath)) + c_executableExtension;

            if (!Path.IsPathRooted(resultPath) && !string.IsNullOrEmpty(executableName))
            {
                // We might find the required executable in the PATH environment folder:
                string composedPath = null;

                foreach (string environmentPath in Environment.GetEnvironmentVariable("PATH").Split(';'))
                {
                    composedPath = $@"{environmentPath}\{resultPath}";

                    if (File.Exists($@"{composedPath}\{executableName}"))
                    {
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(composedPath))
                {
                    resultPath = composedPath;
                }
            }

            return resultPath;
        }

        public override bool IsValidValue(string path)
        {
            if (base.IsValidValue(path))
            {
                return true;
            }

            // Try to find the given path using the 'PATH' environment variable:
            string strippedPath = Utils.StripPath(path) + c_executableExtension;

            return Environment.GetEnvironmentVariable("PATH").Split(';').Any(
                folder => File.Exists(Path.Combine(folder, strippedPath)));
        }
    }
}