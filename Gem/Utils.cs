using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Gem
{
    public static class Utils
    {
        private static readonly Regex s_quotedPathRegex = new Regex("\"(?<innerPath>.*)\"");

        /// <summary>
        /// Verify a given parameter is not null.
        /// </summary>
        /// <param name="parameter">The verified parameter.</param>
        /// <param name="name">The name of the parameter verified.</param>
        public static void GuardNotNull(object parameter, string name)
        {
            if (parameter == null)
            {
                throw new NullReferenceException(string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' cannot be null", name));
            }
        }

        /// <summary>
        /// Since <see cref="Path"/> methods do not understand when the given path is quoted,
        /// This is a helper method to get just the file name out of a given path.
        /// </summary>
        /// <param name="path">The path to extract the file name from.</param>
        /// <returns>The required file name.</returns>
        public static string GetFileName(string path)
        {
            return Path.GetFileName(StripPath(path));
        }

        /// <summary>
        /// Since <see cref="Path"/> methods do not understand when the given path is quoted,
        /// This is a helper method to strip the quotes, if there are any.
        /// </summary>
        /// <param name="path">The path in question.</param>
        /// <returns>The path stripped of potential quotes.</returns>
        public static string StripPath(string path)
        {
            var matchQuoted = s_quotedPathRegex.Match(path);
            return matchQuoted.Success
                ? matchQuoted.Groups["innerPath"].Value
                : path;
        }
    }
}
