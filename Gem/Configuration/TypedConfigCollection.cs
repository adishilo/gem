using System.Configuration;

namespace Gem.Configuration
{
    public abstract class TypedConfigCollection<TElement> : ConfigurationElementCollection
        where TElement : ConfigurationElement, new()
    {
        #region ConfigurationElementCollection overrides

        protected override ConfigurationElement CreateNewElement()
        {
            return new TElement();
        }

        #endregion

        #region Collection operations

        public TElement this[int index]
        {
            get
            {
                return (TElement)BaseGet(index);
            }

            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        public TElement this[object key] => (TElement)BaseGet(key);

        public void Add(TElement typedConfig)
        {
            BaseAdd(typedConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(TElement typedConfig)
        {
            BaseRemove(GetElementKey(typedConfig));
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        #endregion
    }
}
