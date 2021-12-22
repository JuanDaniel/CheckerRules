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
        public Guid Id { get => new Guid(); }

        public string Name { get => "Not Placed"; }

        public string Group { get => "Architecture"; }

        public void Execute(Document document, SLDocument excel)
        {
            //throw new NotImplementedException();
        }
    }

    public class Rule2 : ICheckerRule
    {
        public Guid Id { get => new Guid(); }

        public string Name { get => "Not Enclosed"; }

        public string Group { get => "Architecture"; }

        public void Execute(Document document, SLDocument excel)
        {
            //throw new NotImplementedException();
        }
    }

    public class Rule3 : ICheckerRule
    {
        public Guid Id { get => new Guid(); }

        public string Name { get => "Redundant"; }

        public string Group { get => "Architecture"; }

        public void Execute(Document document, SLDocument excel)
        {
            //throw new NotImplementedException();
        }
    }
}
