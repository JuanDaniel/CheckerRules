using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BBI.JD.Util;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var form = new BBI.JD.Forms.Form2())
            {
                form.ShowDialog();
            }

            /*ResolveDependencies();
            Core.LoadAddin(@"D:\CODE\Visual Studio 2017\Projects\CheckerRules\CheckerRules\bin\Debug\CheckerRules.dll", true);*/
        }

        private static void ResolveDependencies()
        {
            Dictionary<string, string> dependencies = new Dictionary<string, string>()
            {
                { "RevitAPI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null", @"C:\Program Files\Autodesk\Revit 2019\RevitAPI.dll" },
                { "RevitAPIUI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=null", @"C:\Program Files\Autodesk\Revit 2019\RevitAPIUI.dll" },

                { "RevitAPI, Version=21.0.0.0, Culture=neutral, PublicKeyToken=null", @"C:\Program Files\Autodesk\Revit 2021\RevitAPI.dll" },
                { "RevitAPIUI, Version=21.0.0.0, Culture=neutral, PublicKeyToken=null", @"C:\Program Files\Autodesk\Revit 2021\RevitAPIUI.dll" },
            };

            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs e)
            {
                if (dependencies.ContainsKey(e.Name))
                {
                    return Assembly.LoadFrom(dependencies[e.Name]);
                }

                return null;
            };
        }
    }
}
