using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Threading;
using Autodesk.Revit.DB;
using SpreadsheetLight;

namespace BBI.JD.Util
{
    public static class Core
    {
        /// <summary>
        /// Load DLL addin containing rules and add these to colletion rules loaded in <c>CheckerRules.config</c>
        /// </summary>
        /// <param name="path">Absolute path to DLL</param>
        /// <param name="force">Boolean to force reload DLL</param>
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

                foreach (Type type in asm.GetICheckerRuleTypes())
                {
                    var obj = (ICheckerRule)Activator.CreateInstance(type);
                    
                    if (rulesId.ContainsKey(obj.Id))
                    {
                        throw new RepeatedRuleIdException("The addin has duplicates ID rules.");
                    }

                    addin.Rules.Add(new Rule(obj.Id, obj.Name, obj.Group, obj.Description, type.AssemblyQualifiedName, addin));

                    rulesId.Add(obj.Id, obj.Name);
                }

                Config.AddAddin(addin);
            }
        }

        /// <summary>
        /// Return only loadable types from assembly
        /// </summary>
        /// <param name="path">Assembly previously loaded</param>
        /// <returns>Returns the types it implements <c>ICheckerRule</c> interface</returns>
        public static IEnumerable<Type> GetICheckerRuleTypes(this Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException(nameof(asm));
            }

            try
            {
                return asm.GetTypes()
                    .Where(x => typeof(ICheckerRule).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);
            }
            /*catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }*/
            catch (Exception ex)
            {
                return Enumerable.Empty<Type>();
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

        /// <summary>
        /// Prepare an <c>ICheckerRule</c> instance from <c>RulesElement</c> given
        /// </summary>
        /// <param name="rule">Entry <c>RulesElement</c> from CheckerRules.config</param>
        /// <returns>Return an <c>ICheckerRule</c> instance ready to use</returns>
        public static ICheckerRule GetInstance(RulesElement rule)
        {
            AddinsElement addin = rule.Parent.Parent;
            Assembly asm = Assembly.LoadFrom(addin.Path);

            try
            {
                var obj = Activator.CreateInstance(asm.FullName, Type.GetType(rule.AssemblyQualifiedName).ToString());

                return (ICheckerRule)obj.Unwrap();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void VerifySecurityEvidenceForIsolatedStorage(Assembly assembly)
        {
            var isEvidenceFound = true;
            var initialAppDomainEvidence = Thread.GetDomain().Evidence;

            try
            {
                // this will fail when the current AppDomain Evidence is instantiated via COM or in PowerShell
                using (var usfdAttempt1 = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForDomain())
                {
                }
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException e)
            {
                isEvidenceFound = false;
            }

            if (!isEvidenceFound)
            {
                initialAppDomainEvidence.AddHostEvidence(new Url(assembly.Location));
                initialAppDomainEvidence.AddHostEvidence(new Zone(SecurityZone.MyComputer));

                var currentAppDomain = Thread.GetDomain();
                var securityIdentityField = currentAppDomain.GetType().GetField("_SecurityIdentity", BindingFlags.Instance | BindingFlags.NonPublic);
                securityIdentityField.SetValue(currentAppDomain, initialAppDomainEvidence);

                var latestAppDomainEvidence = Thread.GetDomain().Evidence;
            }
        }

        private static void ExportExcel()
        {
            VerifySecurityEvidenceForIsolatedStorage(Assembly.GetExecutingAssembly());

            using (SLDocument sl = new SLDocument())
            {
                //sl.ImportDataTable()
            }
        }

        public static void Execute(Document document, List<ICheckerRule> rules, bool fromLoadLinks = false)
        {
            foreach (ICheckerRule rule in rules)
            {
                try
                {
                    DataTable result = rule.Execute(document);
                }
                catch (NotImplementedException ex)
                {

                }
                catch (Exception ex)
                {

                }
            }
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
        private string Description;
        private string AssemblyQualifiedName;
        private Addin Addin;

        public Rule(string Id, string Name, string Group, string Description, string AssemblyQualifiedName, Addin Addin) {
            this.Id = Id;
            this.Name = Name;
            this.Group = Group;
            this.Description = Description;
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
            rule.Description = Description;
            rule.AssemblyQualifiedName = AssemblyQualifiedName;

            return rule;
        }
    }

    public class RepeatedRuleIdException : Exception {
        public RepeatedRuleIdException(string message) : base(message){}
    }
}