using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BBI.JD.Util;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ResolveDependencies();

            /*using (var form = new BBI.JD.Forms.Form2())
            {
                form.ShowDialog();
            }*/

            Core.LoadAddin(@"D:\WORK\Visual Studio Projects - 2017\Projects\CheckerRules\AR_Rules\bin\Debug\AR_Rules.dll", true);
        }

        private static void ResolveDependencies()
        {
            Assembly.LoadFrom(@"C:\Program Files\Autodesk\Revit 2019\RevitAPI.dll");
            //Assembly.LoadFrom(@"C:\Program Files\Autodesk\Revit 2019\RevitAPIUI.dll");

            /*Dictionary<string, string> dependencies = new Dictionary<string, string>()
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
            };*/
        }
    }
}
