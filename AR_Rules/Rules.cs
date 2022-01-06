using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SpreadsheetLight;
using BBI.JD;

namespace AR_Rules
{
    public class Rule1 : ICheckerRule
    {
        public string Id { get => "553a06e6-9bba-43b4-9f5f-7753b79d3e83"; }

        public string Name { get => "Not Placed"; }

        public string Group { get => "Architecture"; }

        public void Execute(Document document, SLDocument excel)
        {
            //throw new NotImplementedException();
        }
    }

    public class Rule2 : ICheckerRule
    {
        public String Id { get => "71597df6-875b-4680-b209-d6a1f5294608"; }

        public string Name { get => "Not Enclosed"; }

        public string Group { get => "Architecture"; }

        public void Execute(Document document, SLDocument excel)
        {
            //throw new NotImplementedException();
        }
    }

    public class Rule3 : ICheckerRule
    {
        public string Id { get => "418c324d-99e9-473f-9277-654a5cada65b"; }

        public string Name { get => "Redundant"; }

        public string Group { get => "Architecture"; }

        public void Execute(Document document, SLDocument excel)
        {
            //throw new NotImplementedException();
        }
    }
}
