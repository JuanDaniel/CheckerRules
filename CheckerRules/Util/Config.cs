using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;

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

        public static RulesLoadedCollection GetRulesLoaded()
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                CheckerRulesConfig section = (CheckerRulesConfig)config.GetSection("checkerRules");

                if (section != null)
                {
                    return section.RulesLoaded;
                }
            }

            return new RulesLoadedCollection();
        }

        public static void SetRulesAddin(List<RuleAddin> rules)
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                CheckerRulesConfig section = (CheckerRulesConfig)config.GetSection("checkerRules");

                if (section != null)
                {
                    RulesLoadedCollection rulesAddin = new RulesLoadedCollection();

                    foreach (var rule in rules)
                    {
                        RulesLoadedElement element = new RulesLoadedElement();
                        element.Id = rule.Id;
                        element.Version = rule.Version;
                        element.Path = rule.Path;

                        rulesAddin.Add(element);
                    }

                    section.RulesLoaded = rulesAddin;

                    config.Save(ConfigurationSaveMode.Modified);
                }
            }
        }

        public static void RemoveRulesFromAddin(string path)
        {
            Configuration config = GetConfiguration();

            if (config != null)
            {
                CheckerRulesConfig section = (CheckerRulesConfig)config.GetSection("checkerRules");

                if (section != null)
                {
                    RulesLoadedCollection rulesAddin = new RulesLoadedCollection();

                    foreach (var element in GetRulesLoaded().Cast<RulesLoadedElement>().Where(x => x.Path != path))
                    {
                        rulesAddin.Add(element);
                    }

                    section.RulesLoaded = rulesAddin;

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

                config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            }
            catch (Exception ex) { }

            return config;
        }
    }

    public class RulesLoadedElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsKey = true, IsRequired = true)]
        public Guid Id
        {
            get { return (Guid)this["id"]; }
            set { this["id"] = value; }
        }
        [ConfigurationProperty("version")]
        public Version Version
        {
            get { return (Version)this["version"]; }
            set { this["version"] = value; }
        }
        [ConfigurationProperty("path")]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }

    [ConfigurationCollection(typeof(RulesLoadedElement), AddItemName = "update")]
    public class RulesLoadedCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RulesLoadedElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RulesLoadedElement)element).Id;
        }

        public void Add(RulesLoadedElement element)
        {
            BaseAdd(element);
        }
    }

    public class CheckerRulesConfig : ConfigurationSection
    {
        [ConfigurationProperty("rulesLoaded", IsDefaultCollection = true)]
        public RulesLoadedCollection RulesLoaded
        {
            get { return (RulesLoadedCollection)this["rulesLoaded"]; }
            set { this["rulesLoaded"] = value; }
        }
    }
}
