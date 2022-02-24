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
    public class NOT_PLACED : ICheckerRule
    {
        public string Id { get => "553a06e6-9bba-43b4-9f5f-7753b79d3e83"; }

        public string Name { get => "Not Placed"; }

        public string Group { get => "Architecture"; }

        public string Description { get => "Find ROOM & AREA not placed."; }

        public List<Dictionary<string, string>> Execute(Document document)
        {
            List<Dictionary<string, string>> elements = new List<Dictionary<string, string>>();

            foreach (ElementType type in new ElementType[] { ElementType.ROOM, ElementType.AREA })
            {
                foreach (SpatialElement element in Core.GetData(document, type, new ElementValue[] { ElementValue.NOT_PLACED }))
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();

                    data.Add("ID", element.Id.ToString());
                    data.Add("Type", type.ToString());
                    data.Add("Name", element.Name);
                    data.Add("Level", element.Level != null ? element.Level.Name : "");
                    //data.Add("Phase", element.LookupParameter("Phase"));
                    data.Add("File", element.Document.PathName);

                    elements.Add(data);
                }
            }

            return elements;
        }
    }

    public class Rule2 : ICheckerRule
    {
        public String Id { get => "71597df6-875b-4680-b209-d6a1f5294608"; }

        public string Name { get => "Not Enclosed"; }

        public string Group { get => "Architecture"; }

        public string Description { get => "Find ROOM & AREA not enclosed."; }

        public List<Dictionary<string, string>> Execute(Document document)
        {
            throw new NotImplementedException();
        }
    }

    public class Rule3 : ICheckerRule
    {
        public string Id { get => "418c324d-99e9-473f-9277-654a5cada65b"; }

        public string Name { get => "Redundant"; }

        public string Group { get => "Architecture"; }

        public string Description { get => "Find ROOM & AREA redundant."; }

        public List<Dictionary<string, string>> Execute(Document document)
        {
            throw new NotImplementedException();
        }
    }
}
