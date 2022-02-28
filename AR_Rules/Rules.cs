using System;
using System.Collections.Generic;
using System.Data;
using Autodesk.Revit.DB;
using BBI.JD;

namespace AR_Rules
{
    public class NOT_PLACED : ICheckerRule
    {
        public string Id { get => "553a06e6-9bba-43b4-9f5f-7753b79d3e83"; }

        public string Name { get => "Not Placed"; }

        public string Group { get => "Architecture"; }

        public string Description { get => "Find ROOM & AREA not placed."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID"),
                new DataColumn("Type"),
                new DataColumn("Name"),
                new DataColumn("Level"),
                new DataColumn("Phase"),
                new DataColumn("File")
            });

            foreach (ElementType type in new ElementType[] { ElementType.ROOM, ElementType.AREA })
            {
                foreach (SpatialElement element in Core.GetData(document, type, new ElementValue[] { ElementValue.NOT_PLACED }))
                {
                    DataRow row = table.NewRow();

                    row["ID"] = element.Id.ToString();
                    row["Type"] = type.ToString();
                    row["Name"] = element.Name;
                    row["Level"] = element.Level != null ? element.Level.Name : "";

                    if (type == ElementType.ROOM)
                    {
                        row["Phase"] = element.get_Parameter(BuiltInParameter.ROOM_PHASE);
                    }
                    
                    row["File"] = element.Document.PathName;

                    table.Rows.Add(row);
                }
            }

            return table;
        }
    }

    public class NOT_ENCLOSED : ICheckerRule
    {
        public string Id { get => "71597df6-875b-4680-b209-d6a1f5294608"; }

        public string Name { get => "Not Enclosed"; }

        public string Group { get => "Architecture"; }

        public string Description { get => "Find ROOM & AREA not enclosed."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID"),
                new DataColumn("Type"),
                new DataColumn("Name"),
                new DataColumn("Level"),
                new DataColumn("Phase"),
                new DataColumn("File")
            });

            foreach (ElementType type in new ElementType[] { ElementType.ROOM, ElementType.AREA })
            {
                foreach (SpatialElement element in Core.GetData(document, type, new ElementValue[] { ElementValue.NOT_ENCLOSED }))
                {
                    DataRow row = table.NewRow();

                    row["ID"] = element.Id.ToString();
                    row["Type"] = type.ToString();
                    row["Name"] = element.Name;
                    row["Level"] = element.Level != null ? element.Level.Name : "";
                    //row["Phase"] = 
                    row["File"] = element.Document.PathName;

                    table.Rows.Add(row);
                }
            }

            return table;
        }
    }

    public class REDUNDANT : ICheckerRule
    {
        public string Id { get => "418c324d-99e9-473f-9277-654a5cada65b"; }

        public string Name { get => "Redundant"; }

        public string Group { get => "Architecture"; }

        public string Description { get => "Find ROOM & AREA redundant."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("ID"),
                new DataColumn("Type"),
                new DataColumn("Name"),
                new DataColumn("Level"),
                new DataColumn("Phase"),
                new DataColumn("File")
            });

            foreach (ElementType type in new ElementType[] { ElementType.ROOM, ElementType.AREA })
            {
                foreach (SpatialElement element in Core.GetData(document, type, new ElementValue[] { ElementValue.REDUNDANT }))
                {
                    DataRow row = table.NewRow();

                    row["ID"] = element.Id.ToString();
                    row["Type"] = type.ToString();
                    row["Name"] = element.Name;
                    row["Level"] = element.Level != null ? element.Level.Name : "";
                    //row["Phase"] = 
                    row["File"] = element.Document.PathName;

                    table.Rows.Add(row);
                }
            }

            return table;
        }
    }
}
