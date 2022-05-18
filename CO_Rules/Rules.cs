using Autodesk.Revit.DB;
using BBI.JD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_Rules
{
    public class WORKSETS : ICheckerRule
    {
        public string Id { get => "b1ed2dac-d5d3-4b7f-bcdd-50fb2dc9a93f"; }

        public string Name { get => "Worksets"; }

        public string Group { get => "Collaborate"; }

        public string Description { get => "Get all worksets."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("Name"),
                new DataColumn("Type"),
                new DataColumn("Editable"),
                new DataColumn("Owner"),
                new DataColumn("Opened"),
                new DataColumn("Visible in all views"),
                new DataColumn("File")
            });

            WorksetTable worksetTable = document.GetWorksetTable();

            foreach (Workset workset in new FilteredWorksetCollector(document)
                .Cast<Workset>().Where(x => x.Kind == WorksetKind.UserWorkset)) // filter only user workset
            {
                DataRow row = table.NewRow();
                row["Name"] = workset.Name;
                row["Type"] = workset.Kind;
                row["Editable"] = workset.IsEditable ? "YES" : "NO";
                row["Owner"] = workset.Owner;
                row["Opened"] = workset.IsOpen ? "YES" : "NO";
                row["Visible in all views"] = workset.IsVisibleByDefault ? "YES" : "NO";
                row["File"] = document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
