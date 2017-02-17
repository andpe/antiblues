using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AntiBlues.Helpers
{
    class Config
    {
        private string mConfigPath;
        private XDocument mConfig;
        private XElement mRoot;

        public Config(string path)
        {
            mConfigPath = path;
            this.mConfig = XDocument.Load(path);
            this.mRoot = this.mConfig.Element("configfile");
        }

        public Config(string path, XDocument config)
        {
            mConfigPath = path;
            mConfig = config;
            mRoot = config.Element("configfile");
        }

        /// <summary>
        /// Get a value from the config.
        /// </summary>
        /// <typeparam name="T">Type to cast to, must implement IConvertable</typeparam>
        /// <param name="name">Name of the config key</param>
        /// <param name="default_value">Default value of this config</param>
        /// <returns>Value from the config file</returns>
        public T getConfig<T>(string name, T default_value)
        {
            this.mConfig = XDocument.Load(mConfigPath);
            T res = default_value;

            Type t = typeof(T);
            XElement conf = mRoot.Element(name);

            string val = conf?.Value;
            if(val != null)
                res = (T)Convert.ChangeType(val, typeof(T));

            return res;
        }

        /// <summary>
        /// Saves a value to the config
        /// </summary>
        /// <param name="name">Name of the config key</param>
        /// <param name="value">String value of the element</param>
        public void setConfig(string name, string value)
        {
            XElement e = this.mConfig.Element(name);
            if (e == null) {
                e = new XElement(name);
                e.SetValue(value);
                mRoot.Add(e);
            } else {
                e.SetValue(value);
            }

            this.mConfig.Save(mConfigPath);
        }
    }
}
