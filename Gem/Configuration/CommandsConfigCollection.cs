using System;
using System.Configuration;

namespace Gem.Configuration
{
    public class CommandsConfigCollection : TypedConfigCollection<CommandConfigElement>
    {
        public CommandsConfigCollection()
        {
            AddElementName = "CustomCommand";
        }

        #region ConfigurationElementCollection overrides

        protected override object GetElementKey(ConfigurationElement element)
        {
            var commandElement = element as CommandConfigElement;

            if (commandElement == null)
            {
                throw new ArgumentException("element");
            }

            return commandElement.Name;
        }

        #endregion
    }
}
