using System;
using System.Diagnostics;

namespace Gem
{
    /// <summary>
    /// This class allows the lazy evaluation of it value, only when read-accessed.
    /// The evaluation can be anything injected by the using application.
    /// </summary>
    public class LazyEvalString
    {
        private readonly Func<string, string> m_evaluator;

        public LazyEvalString(string templateValue, Func<string, string> evaluator)
        {
            m_evaluator = evaluator;
            TemplateValue = templateValue;
        }

        /// <summary>
        /// Gets the template values of the string (before evaluation)
        /// </summary>
        public string TemplateValue { get; }

        public static implicit operator string(LazyEvalString inputTemplate)
        {
            return inputTemplate?.m_evaluator(inputTemplate.TemplateValue);
        }

        public static implicit operator LazyEvalString(string input)
        {
            if (input == null)
            {
                return null;
            }

            return new LazyEvalString(input, s => s);
        }

        public override string ToString()
        {
            return m_evaluator(TemplateValue);
        }
    }
}
