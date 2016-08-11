using System;
using System.Collections.Generic;
using System.Text;

namespace Gem
{
    /// <summary>
    /// 1. Allows registration of keywords and the creation of a string consisting of those keywords replaced in the original string.
    /// 2. Maintains a list of input parameters as special keywords in the string that can be accessed later when needed.
    /// </summary>
    public class StringCustomReplacer
    {
        private readonly List<Tuple<string, string>> m_replacements = new List<Tuple<string, string>>(); 

        /// <summary>
        /// Register a replacement for a possible keyword in the base-format.
        /// </summary>
        /// <param name="keyword">The keyword to search (in the format it is prefixed by '$').</param>
        /// <param name="replacement">The replacement for this custom-replacer object</param>
        public void RegisterKeyword(string keyword, string replacement)
        {
            string searchFor = $"${keyword}";

            m_replacements.Add(new Tuple<string, string>(
                searchFor, replacement));
        }

        public string Format(string input)
        {
            StringBuilder creator = new StringBuilder(input);

            foreach (var replacement in m_replacements)
            {
                creator.Replace(replacement.Item1, replacement.Item2);
            }

            return creator.ToString();
        }
    }
}
