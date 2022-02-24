using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace BBI.JD.Util
{
    public static class Config
    {
        static string APPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string CFGFOLDER_PATH = Path.Combine(APPDATA_PATH, "BBI CheckerRules");
        static string CFGFILE_PATH = Path.Combine(CFGFOLDER_PATH, "CheckerRules.config");

        public static string Get(string key)
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];

                if (element != null)
                {
                    return element.Value;
                }
            }

            return string.Empty;
        }

        public static void Set(string key, string value)
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];

                if (element != null)
                {
                    element.Value = value;

                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
        }

        public static AddinsCollection GetAddinsLoaded()
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                CheckerRulesConfig section = (CheckerRulesConfig)config.GetSection("checkerRules");

                if (section != null)
                {
                    return section.Addins;
                }
            }

            return new AddinsCollection();
        }

        public static void AddAddin(Addin addin)
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                CheckerRulesConfig section = (CheckerRulesConfig)config.GetSection("checkerRules");

                if (section != null)
                {
                    section.Addins.Add(addin.ToAddinsElement());

                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
        }

        public static void RemoveAddin(string path)
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                CheckerRulesConfig section = (CheckerRulesConfig)config.GetSection("checkerRules");

                if (section != null)
                {
                    AddinsCollection addins = new AddinsCollection();

                    foreach (var element in GetAddinsLoaded().Cast<AddinsElement>().Where(x => x.Path != path))
                    {
                        addins.Add(element);
                    }

                    section.Addins = addins;

                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
        }

        private static Configuration GetConfiguration()
        {
            Configuration config = null;

            try
            {
                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = CFGFILE_PATH;

                AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs e)
                {
                    if (Assembly.GetExecutingAssembly().GetName().Name == e.Name)
                    {
                        return Assembly.GetExecutingAssembly();
                    }

                    return null;
                };

                config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            }
            catch (Exception ex) { }

            return config;
        }
    }

    public class AddinsElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
        [ConfigurationProperty("version", IsRequired = true)]
        public string Version
        {
            get { return (string)this["version"]; }
            set { this["version"] = value; }
        }
        [ConfigurationProperty("rules", IsRequired = true, IsDefaultCollection = true)]
        public RulesCollection Rules
        {
            get {
                RulesCollection rulesCollection = (RulesCollection)this["rules"];
                rulesCollection.Parent = this;

                return rulesCollection;
            }
            set { this["rules"] = value; }
        }
    }

    [ConfigurationCollection(typeof(AddinsElement), AddItemName = "addin")]
    public class AddinsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AddinsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AddinsElement)element).Path;
        }

        public void Add(AddinsElement element)
        {
            BaseAdd(element);
        }
    }

    public class RulesElement : ConfigurationElement
    {
        internal RulesCollection Parent { get; set; }

        [ConfigurationProperty("id", IsKey = true, IsRequired = true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("group", IsRequired = true)]
        public string Group
        {
            get { return (string)this["group"]; }
            set { this["group"] = value; }
        }
        [ConfigurationProperty("description", IsRequired = true)]
        public string Description
        {
            get { return (string)this["description"]; }
            set { this["description"] = value; }
        }
        [ConfigurationProperty("assemblyQualifiedName", IsRequired = true)]
        public string AssemblyQualifiedName
        {
            get { return (string)this["assemblyQualifiedName"]; }
            set { this["assemblyQualifiedName"] = value; }
        }
    }

    [ConfigurationCollection(typeof(RulesElement), AddItemName = "rule")]
    public class RulesCollection : ConfigurationElementCollection
    {
        internal AddinsElement Parent { get; set; }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RulesElement { Parent = this };
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RulesElement)element).Id;
        }

        public void Add(RulesElement element)
        {
            BaseAdd(element);
        }
    }

    public class CheckerRulesConfig : ConfigurationSection
    {
        [ConfigurationProperty("addins", IsDefaultCollection = true)]
        public AddinsCollection Addins
        {
            get { return (AddinsCollection)this["addins"]; }
            set { this["addins"] = value; }
        }
    }
}
