namespace Gem
{
    public class SccProvidersStaticUtils
    {
        private static readonly object s_syncInstance = new object();

        private static SccProvidersStaticUtils s_instance = null;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static SccProvidersStaticUtils Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (s_syncInstance)
                    {
                        if (s_instance == null)
                        {
                            s_instance = new SccProvidersStaticUtils();
                        }
                    }
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Identify an SCC provider for the given root folder.
        /// </summary>
        /// <param name="rootFolder">The root folder to check for an SCC provider.</param>
        /// <returns>The SCC provider, or 'null' if non exists.</returns>
        public ISccProvider GetProvider(string rootFolder)
        {
            // Check for Git:
            ISccProvider result = new GitProvider(rootFolder);

            if (result.IsFolderConnectedToScc)
            {
                return result;
            }

            return null;
        }
    }
}
