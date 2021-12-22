using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBI.JD.Util;

namespace BBI.JD
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class CheckerRules : IExternalCommand
    {
        private UIApplication application;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            application = commandData.Application;

            try
            {
                Core.LoadAddin(@"D:\CODE\Visual Studio 2017\Projects\CheckerRules\AR_Rules\bin\Debug\AR_Rules.dll");

                //ShowForm();
            }
            catch (Exception ex)
            {
                message = ex.Message;

                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private void ShowForm()
        {
            using (var form = new Forms.Form2())
            {
                form.ShowDialog();
            }
        }
    }
}
