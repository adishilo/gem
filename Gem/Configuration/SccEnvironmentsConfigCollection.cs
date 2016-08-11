using System;
using System.Configuration;

namespace Gem.Configuration
{
    public class SccEnvironmentsConfigCollection : TypedConfigCollection<SccEnvironmentConfigElement>
    {
        public SccEnvironmentsConfigCollection()
        {
            AddElementName = "SccEnvironment";
        }

        #region ConfigurationElementCollection overrides

        protected override object GetElementKey(ConfigurationElement element)
        {
            var sccEnvironmentElement = element as SccEnvironmentConfigElement;

            if (sccEnvironmentElement == null)
            {
                throw new ArgumentException("element");
            }

            return sccEnvironmentElement.Folder;
        }

        #endregion
    }
}
