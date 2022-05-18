using Autodesk.Revit.DB;
using BBI.JD;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

        public string Description { get => "Get information from associated links (RVT, IFC, CAD, PCLOUDS)."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("Host"),
                new DataColumn("Type"),
                new DataColumn("Link Name"),
                //new DataColumn("Status"), // not always works for nested links
                new DataColumn("Reference Type"),
                new DataColumn("Instances"),
                new DataColumn("Positions Not Saved"),
                new DataColumn("Path Type"),
                new DataColumn("Size"),
                new DataColumn("File"),
            });

            RVTLinksType(document, table); // RVT or IFC
            CADLinksType(document, table);
            PointCloudType(document, table);

            return table;
        }

        private void RVTLinksType(Document document, DataTable table)
        {
            FileInfo host = new FileInfo(document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName);

            foreach (RevitLinkType link in new FilteredElementCollector(document)
                    .OfClass(typeof(RevitLinkType)))
            {
                // only the root links
                if (link.GetParentId() == ElementId.InvalidElementId)
                {
                    DataRow row = table.NewRow();
                    row["Host"] = host.Name;
                    row["Type"] = link.Name.ToLower().EndsWith(".ifc") ? "IFC" : "RVT";
                    row["Link Name"] = link.Name;
                    //row["Status"] = RevitLinkType.IsLoaded(document, link.Id) ? "Loaded" : "Unloaded";
                    row["Reference Type"] = link.AttachmentType.ToString();
                    row["Instances"] = new FilteredElementCollector(document)
                        .WherePasses(new ElementClassFilter(typeof(RevitLinkInstance))).Where(x => x.GetTypeId() == link.GetTypeId()).Count();
                    row["Positions Not Saved"] = link.HasSaveablePositions() ? "Yes" : "No";
                    row["Path Type"] = link.PathType.ToString();

                    string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(link.GetExternalFileReference().GetAbsolutePath());

                    if (File.Exists(path))
                    {
                        FileInfo info = new FileInfo(path);
                        row["Size"] = ToSize(info.Length, SizeUnits.MB, true);
                        row["File"] = path;
                    }
                    else
                    {
                        row["File"] = "NOT FOUND";
                    }

                    table.Rows.Add(row);
                }
            }
        }

        private void CADLinksType(Document document, DataTable table)
        {
            FileInfo host = new FileInfo(document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName);

            foreach (CADLinkType link in new FilteredElementCollector(document)
                    .OfClass(typeof(CADLinkType)))
            {
                DataRow row = table.NewRow();
                row["Host"] = host.Name;
                row["Type"] = "CAD";
                row["Link Name"] = link.Name;
                //row["Status"] = RevitLinkType.IsLoaded(document, link.Id) ? "Loaded" : "Unloaded";
                row["Instances"] = new FilteredElementCollector(document)
                    .WherePasses(new ElementClassFilter(typeof(ImportInstance))).Where(x => x.GetTypeId() == link.GetTypeId()).Count();
                row["Path Type"] = link.GetExternalFileReference().PathType.ToString();

                string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(link.GetExternalFileReference().GetAbsolutePath());

                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    row["Size"] = ToSize(info.Length, SizeUnits.MB, true);
                    row["File"] = path;
                }
                else
                {
                    row["File"] = "NOT FOUND";
                }

                table.Rows.Add(row);
            }
        }

        private void PointCloudType(Document document, DataTable table)
        {
            FileInfo host = new FileInfo(document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName);

            foreach (PointCloudType link in new FilteredElementCollector(document)
                    .OfClass(typeof(PointCloudType)))
            {
                DataRow row = table.NewRow();
                row["Host"] = host.Name;
                row["Type"] = link.Name.ToLower().EndsWith(".rcp") ? "RCP" : "RCS";
                row["Link Name"] = link.Name;
                //row["Status"] = RevitLinkType.IsLoaded(document, link.Id) ? "Loaded" : "Unloaded";
                row["Instances"] = new FilteredElementCollector(document)
                    .WherePasses(new ElementClassFilter(typeof(PointCloudType))).Where(x => x.GetTypeId() == link.GetTypeId()).Count();

                string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(link.GetPath());

                // for relative path
                if (path.StartsWith("."))
                {
                    path = FullPathRelativeTo(host.DirectoryName, path);

                    row["Path Type"] = "Relative";
                }
                else
                {
                    row["Path Type"] = "Absolute";
                }

                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    row["Size"] = ToSize(info.Length, SizeUnits.MB, true);
                    row["File"] = path;
                }
                else
                {
                    row["File"] = "NOT FOUND";
                }

                table.Rows.Add(row);
            }
        }

        private enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        private string ToSize(long value, SizeUnits unit, bool unitSuffix = false)
        {
            string size = (value / Math.Pow(1024, (long)unit)).ToString("0.00");

            if (unitSuffix)
            {
                size = string.Format("{0} {1}", size, unit);
            }

            return size;
        }

        private string FullPathRelativeTo(string root, string partialPath)
        {
            string currentRoot = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(root);

                return Path.GetFullPath(partialPath);
            }
            catch
            {
                return partialPath;
            }
            finally
            {
                Directory.SetCurrentDirectory(currentRoot);
            }
        }
    }

    public class TEMPLATE_REVIEW : ICheckerRule
    {
        public string Id { get => "6d255b9d-c13d-463d-bd08-2bd649ad0978"; }

        public string Name { get => "Template Review"; }

        public string Group { get => "Manage"; }

        public string Description { get => "Get review date from BBI Cuba template."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("Date"),
                new DataColumn("File")
            });

            FilteredElementCollector titleBlocks = new FilteredElementCollector(document)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilyInstance));

            DataRow row = table.NewRow();

            row["Date"] = "INVALID TEMPLATE";
            row["File"] = document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName;

            foreach (FamilyInstance block in titleBlocks)
            {
                Parameter param = block.get_Parameter(BuiltInParameter.SHEET_NUMBER);

                if (param != null)
                {
                    if (param.AsString() == "0") // SheetNumber == "0" is D94.HOME.1
                    {
                        param = block.get_Parameter(BuiltInParameter.SHEET_ISSUE_DATE);

                        row["Date"] = param.AsString();

                        break;
                    }
                }
            }

            table.Rows.Add(row);

            return table;
        }
    }

    public class WARNINGS : ICheckerRule
    {
        public string Id { get => "6d255b9d-c13d-463d-bd08-2bd649ad0962"; }

        public string Name { get => "Warnings"; }

        public string Group { get => "Manage"; }

        public string Description { get => "Get warnings."; }

        private MessageGrouped WarningGrouped;

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("Priority"),
                new DataColumn("Warning"),
                new DataColumn("Element Ids"),
                new DataColumn("File")
            });

            IList<FailureMessage> warnings = document.GetWarnings();

            foreach (FailureMessage warning in warnings)
            {
                DataRow row = table.NewRow();
                row["Priority"] = GetPriority(warning.GetDescriptionText());
                row["Warning"] = warning.GetDescriptionText();
                row["Element Ids"] = string.Join(", ", warning.GetFailingElements());
                row["File"] = document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName;
                table.Rows.Add(row);
            }

            return table;
        }

        private string GetPriority(string message)
        {
            // check Revit language
            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != "en") {
                return "- UNCLASSIFIED -";
            }

            MessageGrouped messages = ReadWarningGrouped();

            if (messages.CRITICAL.Contains(message))
            {
                return "CRITICAL";
            }
            if (messages.HIGH.Contains(message))
            {
                return "HIGH";
            }
            if (messages.MEDIUM.Contains(message))
            {
                return "MEDIUM";
            }
            if (messages.LOW.Contains(message))
            {
                return "LOW";
            }

            return "- UNCLASSIFIED -";
        }

        private MessageGrouped ReadWarningGrouped()
        {
            if (WarningGrouped == null)
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                string folder = new FileInfo(assemblyPath).Directory.FullName;

                using (var fs = new FileStream(string.Concat(folder, "/Resources/WarningGrouped_EN.json"), FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        List<string> file = new List<string>();

                        while (!sr.EndOfStream)
                        {
                            file.Add(sr.ReadLine());
                        }

                        WarningGrouped = JsonConvert.DeserializeObject<MessageGrouped>(string.Join("", file.ToArray()));
                    }
                }
            }

            return WarningGrouped;
        }

        class MessageGrouped
        {
            public List<string> CRITICAL;
            public List<string> HIGH;
            public List<string> MEDIUM;
            public List<string> LOW;
        }
    }

    public class UNPURGED : ICheckerRule
    {
        public string Id { get => "6d255b9d-c13d-463d-bd08-2bd649ad12h2"; }

        public string Name { get => "Unpurged"; }

        public string Group { get => "Manage"; }

        public string Description { get => "Get the number of elements to purge."; }

        public DataTable Execute(Document document)
        {
            DataTable table = new DataTable(Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("Number"),
                //new DataColumn("Element Ids"),
                new DataColumn("File")
            });

            Guid guid = new Guid("e8c63650-70b7-435a-9010-ec97660c1bda"); // PURGE_GUID

            IList<FailureMessage> result = PerformanceAdviser.GetPerformanceAdviser()
                .ExecuteRules(document, PerformanceAdviser.GetPerformanceAdviser().GetAllRuleIds()
                    .Where(x => x.Guid.Equals(guid)).ToList());

            DataRow row = table.NewRow();
            row["Number"] = result.Count > 0 ? result[0].GetFailingElements().Count : 0;
            //row["Element Ids"] = result.Count > 0 ? string.Join(", ", result[0].GetFailingElements()) : "";
            row["File"] = document.IsWorkshared ? ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName;
            table.Rows.Add(row);

            return table;
        }
    }
}
