using Autodesk.Revit.DB;
using BBI.JD;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA_Rules
{
    public class PROJECT_UNITS : ICheckerRule

    {
        public string Id { get => "6d255b9d-c13d-463d-bd08-2bd649ad0843"; }

        public string Name { get => "Project Units"; }

        public string Group { get => "Manage"; }

        public string Description { get => "Get project units config."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("Length (Units)"),
                new DataColumn("Length (Rounding)"),
                new DataColumn("Area (Units)"),
                new DataColumn("Area (Rounding)"),
                new DataColumn("Volume (Units)"),
                new DataColumn("Volume (Rounding)"),
                new DataColumn("Angle (Units)"),
                new DataColumn("Angle (Rounding)"),
                new DataColumn("Slope (Units)"),
                new DataColumn("Slope (Rounding)"),
                new DataColumn("Currency (Units)"),
                new DataColumn("Currency (Rounding)"),
                new DataColumn("Mass Density (Units)"),
                new DataColumn("Mass Density (Rounding)"),
                new DataColumn("File")
            });

            Units units = document.GetUnits();

            FormatOptions fo_length;
            FormatOptions fo_area;
            FormatOptions fo_volume;
            FormatOptions fo_angle;
            FormatOptions fo_slope;
            FormatOptions fo_currency;
            FormatOptions fo_mass;

            #if RVT2019
                fo_length = units.GetFormatOptions(UnitType.UT_Length);
                fo_area = units.GetFormatOptions(UnitType.UT_Area);
                fo_volume = units.GetFormatOptions(UnitType.UT_Volume);
                fo_angle = units.GetFormatOptions(UnitType.UT_Angle);
                fo_slope = units.GetFormatOptions(UnitType.UT_Slope);
                fo_currency = units.GetFormatOptions(UnitType.UT_Currency);
                fo_mass = units.GetFormatOptions(UnitType.UT_Mass);
            #else
                fo_length = units.GetFormatOptions(SpecTypeId.UT_Length);
                fo_area = units.GetFormatOptions(SpecTypeId.UT_Area);
                fo_volume = units.GetFormatOptions(SpecTypeId.UT_Volume);
                fo_angle = units.GetFormatOptions(SpecTypeId.UT_Angle);
                fo_slope = units.GetFormatOptions(SpecTypeId.UT_Slope);
                fo_currency = units.GetFormatOptions(SpecTypeId.UT_Currency);
                fo_mass = units.GetFormatOptions(SpecTypeId.UT_Mass);
            #endif

                DataRow row = table.NewRow();
                row["Length (Units)"] = LabelUtils.GetLabelFor(fo_length.DisplayUnits);
                row["Length (Rounding)"] = GetLabelRounding(fo_length.Accuracy);
                row["Area (Units)"] = LabelUtils.GetLabelFor(fo_area.DisplayUnits);
                row["Area (Rounding)"] = GetLabelRounding(fo_area.Accuracy);
                row["Volume (Units)"] = LabelUtils.GetLabelFor(fo_volume.DisplayUnits);
                row["Volume (Rounding)"] = GetLabelRounding(fo_volume.Accuracy);
                row["Angle (Units)"] = LabelUtils.GetLabelFor(fo_angle.DisplayUnits);
                row["Angle (Rounding)"] = GetLabelRounding(fo_angle.Accuracy);
                row["Slope (Units)"] = LabelUtils.GetLabelFor(fo_slope.DisplayUnits);
                row["Slope (Rounding)"] = GetLabelRounding(fo_slope.Accuracy);
                row["Currency (Units)"] = LabelUtils.GetLabelFor(fo_currency.DisplayUnits);
                row["Currency (Rounding)"] = GetLabelRounding(fo_currency.Accuracy);
                row["Mass Density (Units)"] = LabelUtils.GetLabelFor(fo_mass.DisplayUnits);
                row["Mass Density (Rounding)"] = GetLabelRounding(fo_mass.Accuracy);
                row["File"] = document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName;
                table.Rows.Add(row);

            return table;
        }

        private string GetLabelRounding(double accuracy)
        {
            switch (Math.Round(accuracy, 3))
            {
                case 1000:
                    return string.Format("To nearest {0}", accuracy);
                case 100:
                    return string.Format("To nearest {0}", accuracy);
                case 10:
                    return string.Format("To nearest {0}", accuracy);
                case 1:
                    return "0 decimal places";
                case 0.1:
                    return "1 decimal places";
                case 0.01:
                    return "2 decimal places";
                case 0.001:
                    return "3 decimal places";
                default:
                    return string.Format("Custom {0}", accuracy);
            }
        }
    }

    public class LINKS : ICheckerRule
    {
        public string Id { get => "6d255b9d-c13d-463d-bd08-2bd649ad0852"; }

        public string Name { get => "Links"; }

        public string Group { get => "Manage"; }

        public string Description { get => "Get information from associated links."; }

        public DataTable Execute(Document document)
        {
            throw new NotImplementedException();
        }
    }

    public class WARNINGS : ICheckerRule
    {
        public string Id { get => "6d255b9d-c13d-463d-bd08-2bd649ad0962"; }

        public string Name { get => "Warnings"; }

        public string Group { get => "Manage"; }

        public string Description { get => "Get warnings."; }

        public DataTable Execute(Document document)
        {
            throw new NotImplementedException();
        }
    }
}
