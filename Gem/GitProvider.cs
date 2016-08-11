using System.Globalization;
using LibGit2Sharp;

namespace Gem
{
    public class GitProvider : ISccProvider
    {
        private const string c_providerName = "Git";
        private const string c_noRepositoryAvailableHeadName = "<No Git Repository>";

        public GitProvider(string localFolder)
        {
            LocalFolderName = localFolder;
        }

        #region ISccProvider implementation

        public string Name
        {
            get
            {
                return c_providerName;
            }
        }

        public string LocalFolderName { get; private set; }

        /// <summary>
        /// Gets information regarding the branch/commit node pointed by the HEAD, in human readable fashion.
        /// </summary>
        public string FolderContentTitlePresentable
        {
            get
            {
                string result;

                try
                {
                    using (var repo = new Repository(LocalFolderName))
                    {
                        var repoInfo = repo.Info.CurrentOperation == CurrentOperation.None
                            ? string.Empty
                            : "|" + repo.Info.CurrentOperation;

                        result = repo.Info.IsHeadDetached
                            ? string.Format(CultureInfo.InvariantCulture, "(HEAD detached at {0})", repo.Head.Tip.Sha.Substring(0, 7))
                            : repo.Head.Name;

                        result += repoInfo;
                    }
                }
                catch (RepositoryNotFoundException)
                {
                    result = c_noRepositoryAvailableHeadName;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets information regarding the branch/commit node pointed by the HEAD.
        /// </summary>
        public string FolderContentTitle
        {
            get
            {
                try
                {
                    using (var repo = new Repository(LocalFolderName))
                    {
                        return repo.Info.IsHeadDetached
                            ? repo.Head.Tip.Sha
                            : repo.Head.Name;
                    }
                }
                catch (RepositoryNotFoundException)
                {
                    return c_noRepositoryAvailableHeadName;
                }
            }
        }

        public bool IsFolderConnectedToScc
        {
            get
            {
                try
                {
                    new Repository(LocalFolderName).Dispose();

                    return true;
                }
                catch (RepositoryNotFoundException)
                {
                    return false;
                }
            }
        }

        public string InfoTypeDescription
        {
            get
            {
                return "Branch Name";
            }
        }

        #endregion
    }
}
