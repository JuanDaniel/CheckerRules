using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BBI.JD.Util
{
    public static class Core
    {
        /// <summary>
        /// Load DLL addin containing rules and add these to colletion rules loaded in <c>CheckerRules.config</c>
        /// </summary>
        /// <param name="path">Absolute path to DLL</param>
        /// <param name="force">Absolute path to DLL</param>
        public static void LoadAddin(string path, bool force = true)
        {
            if (File.Exists(path))
            {
                if (IsAddinLoaded(path))
                {
                    if (force)
                    {
                        Config.RemoveAddin(path);
                    }
                    else
                    {
                        return;
                    }
                }

                Assembly asm = Assembly.LoadFrom(path);

                Addin addin = new Addin(path, asm.GetName().Version);

                Dictionary<string, string> rulesId = new Dictionary<string, string>();

                foreach (var type in asm.GetTypes().Where(x => !x.IsAbstract))
                {
                    dynamic obj = Activator.CreateInstance(type);

                    if (obj is ICheckerRule)
                    {
                        if (rulesId.ContainsKey(((ICheckerRule)obj).Id))
                        {
                            throw new RepeatedRuleIdException("The addin has duplicates ID rules.");
                        }

                        addin.Rules.Add(new Rule(((ICheckerRule)obj).Id, ((ICheckerRule)obj).Name, ((ICheckerRule)obj).Group, type.AssemblyQualifiedName, addin));

                        rulesId.Add(((ICheckerRule)obj).Id, ((ICheckerRule)obj).Name);
                    }
                }

                Config.AddAddin(addin);
            }
        }

        /// <summary>
        /// Check if DLL addin by path is loaded
        /// </summary>
        /// <param name="path">Absolute path to DLL</param>
        /// <returns>Return a boolean value according addin is loaded or not</returns>
        public static bool IsAddinLoaded(string path)
        {
            return Config.GetAddinsLoaded().Cast<AddinsElement>().FirstOrDefault(x => x.Path == path) != null;
        }

        public static void Execute()
        {

        }
    }

    public class Addin
    {
        private string Path;
        private Version Version;
        public List<Rule> Rules;

        public Addin(string Path, Version Version)
        {
            this.Path = Path;
            this.Version = Version;
            Rules = new List<Rule>();
        }

        /// <summary>
        /// Method to map Addin class to AddinsElement use in Configuration
        /// </summary>
        /// <returns>Return an instance of <c>AddinsElement</c> with map values</returns>
        public AddinsElement ToAddinsElement()
        {
            AddinsElement addin = new AddinsElement();
            addin.Path = Path;
            addin.Version = Version.ToString();
            addin.Rules = new RulesCollection();

            foreach (Rule rule in Rules)
            {
                addin.Rules.Add(rule.ToRulesElement());
            }

            return addin;
        }
    }

    public class Rule
    {
        private string Id;
        private string Name;
        private string Group;
        private string AssemblyQualifiedName;
        private Addin Addin;

        public Rule(string Id, string Name, string Group, string AssemblyQualifiedName, Addin Addin) {
            this.Id = Id;
            this.Name = Name;
            this.Group = Group;
            this.AssemblyQualifiedName = AssemblyQualifiedName;
            this.Addin = Addin;
        }

        /// <summary>
        /// Method to map Rule class to RulesElement use in Configuration
        /// </summary>
        /// <returns>Return an instance of <c>RulesElement</c> with map values</returns>
        public RulesElement ToRulesElement()
        {
            RulesElement rule = new RulesElement();
            rule.Id = Id;
            rule.Name = Name;
            rule.Group = Group;
            rule.AssemblyQualifiedName = AssemblyQualifiedName;

            return rule;
        }
    }

    public class RepeatedRuleIdException : Exception {
        public RepeatedRuleIdException(string message) : base(message){}
    }
}