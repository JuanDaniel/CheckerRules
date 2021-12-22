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
        public static void LoadAddin(string path, bool force = true)
        {
            if (File.Exists(path))
            {
                if (IsAddinLoaded(path) && force)
                {
                    Config.RemoveRulesFromAddin(path);
                }

                // Only for testing propouse but in Revit enviroment all DLL was loaded automatically
                /*AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs e)
                {
                    if (e.Name == "RevitAPI, Version=21.0.0.0, Culture=neutral, PublicKeyToken=null")
                    {
                        return Assembly.LoadFrom(@"C:\Program Files\Autodesk\Revit 2021\RevitAPI.dll");
                    }

                    return null;
                };*/

                Assembly asm = Assembly.LoadFrom(path);

                List<RuleAddin> rules = new List<RuleAddin>();

                foreach (var type in asm.GetTypes())
                {
                    dynamic obj = Activator.CreateInstance(type);

                    if (obj is ICheckerRule)
                    {
                        rules.Add(new RuleAddin(((ICheckerRule)obj).Id, asm.GetName().Version, path));
                    }
                }

                Config.SetRulesAddin(rules);
            }
        }

        public static bool IsAddinLoaded(string path)
        {
            return Config.GetRulesLoaded().Cast<RulesLoadedElement>().FirstOrDefault(x => x.Path == path) != null;
        }
    }

    public class RuleAddin
    {
        public Guid Id;
        public Version Version;
        public string Path;

        public RuleAddin() { }
        public RuleAddin(Guid Id, Version Version, string Path) {
            this.Id = Id;
            this.Version = Version;
            this.Path = Path;
        }
    }
}
