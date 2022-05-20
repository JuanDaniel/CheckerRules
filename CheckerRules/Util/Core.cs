using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Threading;
using Autodesk.Revit.DB;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace BBI.JD.Util
{
    public static class Core
    {
        /// <summary>
        /// Load DLL addin containing rules and add these to colletion rules loaded in <c>CheckerRules.config</c>
        /// </summary>
        /// <param name="path">Absolute path to DLL</param>
        /// <param name="force">Boolean to force reload DLL</param>
        /// <returns>Return an <c>LoadResultType</c> indicating the status of the load operation</returns>
        public static LoadResultType LoadAddin(string path, bool force = true)
        {
            if (File.Exists(path))
            {
                Assembly asm = IsAddinLoaded(path);

                if (asm != null)
                {
                    if (force)
                    {
                        Config.RemoveAddin(path);
                    }
                    else
                    {
                        return LoadResultType.ALREADY_ADDED;
                    }
                }

                asm = Assembly.LoadFrom(path);

                Config.AddAddin(PrepareAddin(asm, path));

                return LoadResultType.SUCCESS;
            }

            return LoadResultType.FILE_NOT_FOUND;
        }

        /// <summary>
        /// Return only loadable types from assembly
        /// </summary>
        /// <param name="path">Assembly previously loaded</param>
        /// <returns>Returns the types it implements <c>ICheckerRule</c> interface</returns>
        public static IEnumerable<Type> GetICheckerRuleTypes(this Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException(nameof(asm));
            }

            try
            {
                return asm.GetTypes()
                    .Where(x => typeof(ICheckerRule).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);
            }
            /*catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }*/
            catch (Exception ex)
            {
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// Check if DLL addin is loaded
        /// </summary>
        /// <param name="path">Absolute path to DLL</param>
        /// <returns>Returns the assembly if it is loaded, otherwise a null value</returns>
        public static Assembly IsAddinLoaded(string path)
        {
            Assembly asm = Assembly.LoadFrom(path);

            GuidAttribute guid = asm.GetCustomAttribute(typeof(GuidAttribute)) as GuidAttribute;

            return Config.GetAddinsLoaded().Cast<AddinsElement>()
                .FirstOrDefault(x => x.Id == guid.Value) != null ? asm : null;
        }

        /// <summary>
        /// Update an addin from its respective new version.
        /// Some validation will be execute to ensure that it is the same addin and that the version provided is superior
        /// </summary>
        /// <param name="currentId">Current ID from addin to update</param>
        /// <param name="path">Absolute path to DLL to replace</param>
        /// <returns>Return an <c>UpdateResultType</c> indicating the status of the update operation</returns>
        public static UpdateResultType UpdateAddin(string currentId, string path)
        {
            AddinsElement currentAddin = Config.GetAddinsLoaded().Cast<AddinsElement>()
                .FirstOrDefault(x => x.Id == currentId);

            Assembly newAddin = Assembly.LoadFrom(path);

            GuidAttribute guid = newAddin.GetCustomAttribute(typeof(GuidAttribute)) as GuidAttribute;

            if (!currentAddin.Id.Equals(guid.Value))
            {
                return UpdateResultType.DIFFERENT_ADDIN;
            }

            Version currentVersion = new Version(currentAddin.Version);

            if (currentVersion.CompareTo(newAddin.GetName().Version) == 0)
            {
                return UpdateResultType.SAME_VERSION;
            }
            if (currentVersion.CompareTo(newAddin.GetName().Version) > 0)
            {
                return UpdateResultType.LESS_VERSION;
            }

            Config.RemoveAddin(currentId);
            Config.AddAddin(PrepareAddin(newAddin, path));

            return UpdateResultType.SUCCESS;
        }

        /// <summary>
        /// Prepare an <c>ICheckerRule</c> instance from <c>RulesElement</c> given
        /// </summary>
        /// <param name="rule">Entry <c>RulesElement</c> from CheckerRules.config</param>
        /// <returns>Return an <c>ICheckerRule</c> instance ready to use</returns>
        public static ICheckerRule GetInstance(RulesElement rule)
        {
            AddinsElement addin = rule.Parent.Parent;
            Assembly asm = Assembly.LoadFrom(addin.Path);

            try
            {
                var obj = Activator.CreateInstance(asm.FullName, Type.GetType(rule.AssemblyQualifiedName).ToString());

                return (ICheckerRule)obj.Unwrap();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Check security evidence to store excel result, if exception throws it resolve these
        /// </summary>
        /// <param name="assembly">Assembly executing</param>
        private static void VerifySecurityEvidenceForIsolatedStorage(Assembly assembly)
        {
            var isEvidenceFound = true;
            var initialAppDomainEvidence = Thread.GetDomain().Evidence;

            try
            {
                // this will fail when the current AppDomain Evidence is instantiated via COM or in PowerShell
                using (var usfdAttempt1 = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForDomain())
                {
                }
            }
            catch (System.IO.IsolatedStorage.IsolatedStorageException e)
            {
                isEvidenceFound = false;
            }

            if (!isEvidenceFound)
            {
                initialAppDomainEvidence.AddHostEvidence(new Url(assembly.Location));
                initialAppDomainEvidence.AddHostEvidence(new Zone(SecurityZone.MyComputer));

                var currentAppDomain = Thread.GetDomain();
                var securityIdentityField = currentAppDomain.GetType().GetField("_SecurityIdentity", BindingFlags.Instance | BindingFlags.NonPublic);
                securityIdentityField.SetValue(currentAppDomain, initialAppDomainEvidence);

                var latestAppDomainEvidence = Thread.GetDomain().Evidence;
            }
        }

        /// <summary>
        /// Prepares the addin with its respective values to add it to the config
        /// </summary>
        /// <param name="asm">Assembly loaded from the addin</param>
        /// <param name="path">Absolute path to DLL</param>
        /// <returns>Return an <c>Addin</c> instance ready to add to the config</returns>
        private static Addin PrepareAddin(Assembly asm, string path)
        {
            GuidAttribute guid = asm.GetCustomAttribute(typeof(GuidAttribute)) as GuidAttribute;

            Addin addin = new Addin(guid.Value, path, asm.GetName().Version);

            Dictionary<string, string> rulesId = new Dictionary<string, string>();

            foreach (Type type in asm.GetICheckerRuleTypes())
            {
                var obj = (ICheckerRule)Activator.CreateInstance(type);

                if (rulesId.ContainsKey(obj.Id))
                {
                    throw new RepeatedRuleIdException("The addin has duplicates ID rules.");
                }

                addin.Rules.Add(new Rule(obj.Id, obj.Name, obj.Group, obj.Description, type.AssemblyQualifiedName, addin));

                rulesId.Add(obj.Id, obj.Name);
            }

            return addin;
        }

        /// <summary>
        /// Prepare DataTable of execution stats to put in HOME worksheet excel result
        /// </summary>
        /// <returns>Return a <c>DataTable</c> with execution stats data</returns>
        private static DataTable DataExecutionStats()
        {
            DataTable table = new DataTable();

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("KEY"),
                new DataColumn("VALUE")
            });

            DataRow row = table.NewRow();
            row["KEY"] = "Start";
            row["VALUE"] = ExecutionStats.Instance.Start;
            table.Rows.Add(row);

            row = table.NewRow();
            row["KEY"] = "End";
            row["VALUE"] = ExecutionStats.Instance.End;
            table.Rows.Add(row);

            // Empty row
            row = table.NewRow();
            row["KEY"] = string.Empty;
            row["VALUE"] = string.Empty;
            table.Rows.Add(row);

            row = table.NewRow();
            row["KEY"] = "Rules";
            row["VALUE"] = string.Empty;
            table.Rows.Add(row);

            foreach (string rule in ExecutionStats.Instance.Rules)
            {
                row = table.NewRow();
                row["KEY"] = string.Empty;
                row["VALUE"] = rule;
                table.Rows.Add(row);
            }

            // Empty row
            row = table.NewRow();
            row["KEY"] = string.Empty;
            row["VALUE"] = string.Empty;
            table.Rows.Add(row);

            row = table.NewRow();
            row["KEY"] = "Files";
            row["VALUE"] = string.Empty;
            table.Rows.Add(row);

            foreach (string file in ExecutionStats.Instance.Files)
            {
                row = table.NewRow();
                row["KEY"] = string.Empty;
                row["VALUE"] = file;
                table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Export excel result of each rule executed
        /// </summary>
        /// <param name="results">Rules's results</param>
        /// <param name="path">Path to store excel</param>
        private static void ExportExcel(Dictionary<ICheckerRule, DataTable> results, string path)
        {
            VerifySecurityEvidenceForIsolatedStorage(Assembly.GetExecutingAssembly());

            using (SLDocument sl = new SLDocument())
            {
                SLStyle h1 = new SLStyle();
                h1.SetFontBold(true);
                h1.SetFont("Calibri", 14);

                SLStyle h2 = new SLStyle();
                h2.SetFontBold(true);
                h2.SetFont("Calibri", 12);

                // HOME sheet
                sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "HOME");
                sl.SetCellValue("B2", "Checker Rules BBI Revit add-in");
                sl.SetCellStyle("B2", h1);
                sl.MergeWorksheetCells(2, 2, 2, 3);
                sl.ImportDataTable("B4", DataExecutionStats(), false);
                sl.AutoFitColumn(2, 3);
                sl.DrawBorder(4, 2, 5 + ExecutionStats.Instance.Rules.Count + ExecutionStats.Instance.Files.Count + 4, 3, BorderStyleValues.Thin);

                // One sheet peer rule results
                foreach (var result in results)
                {
                    sl.AddWorksheet(result.Key.Name);
                    sl.SetCellValue("A1", string.Format("Rule check results ({0})", result.Key.Name));
                    sl.SetCellStyle("A1", h2);
                    sl.MergeWorksheetCells(1, 1, 1, result.Value.Columns.Count);
                    sl.ImportDataTable("A3", result.Value, true);
                    sl.AutoFitColumn(1, result.Value.Columns.Count);

                    SLTable table = sl.CreateTable(3, 1, 3 + result.Value.Rows.Count, result.Value.Columns.Count);
                    table.SetTableStyle(SLTableStyleTypeValues.Medium9);
                    sl.InsertTable(table);
                }

                sl.SelectWorksheet("HOME");
                sl.SaveAs(path);
            }
        }

        /// <summary>
        /// Prepare DataTable for exception rule
        /// </summary>
        /// <param name="rule">Rule executed</param>
        /// <param name="ex">Exception throws</param>
        /// <returns>Return a <c>DataTable</c> with exception message</returns>
        private static DataTable DataExceptionResult(ICheckerRule rule, Exception ex)
        {
            DataTable table = new DataTable(rule.Name);

            table.Columns.AddRange(new DataColumn[] {
                new DataColumn("ERROR")
            });

            DataRow row = table.NewRow();

            row["ERROR"] = ex.Message;

            table.Rows.Add(row);

            return table;
        }

        /// <summary>
        /// Get RVT links inserted into the host and for the sublinks get only the attached links
        /// </summary>
        /// <param name="document">Document from get links</param>
        /// <param name="linksDocument">Links dictionary result use in recursive call</param>
        /// <param name="onlyLoaded">Flag to filter only loaded links</param>
        /// <returns>Return a dictionary compose key path and value document from link</returns>
        private static Dictionary<string, Document> GetRVTLinks(Document document, Dictionary<string, Document> linksDocument, bool onlyLoaded = false)
        {
            foreach (RevitLinkType link in new FilteredElementCollector(document)
                    .OfClass(typeof(RevitLinkType)))
            {
                if (onlyLoaded && !RevitLinkType.IsLoaded(document, link.Id)) {
                    continue;
                }

                string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(link.GetExternalFileReference().GetAbsolutePath());

                if (!linksDocument.ContainsKey(path))
                {
                    Document doc = document.Application.Documents.Cast<Document>()
                            .Where(d => d.PathName == path)
                                .FirstOrDefault();

                    if (doc != null)
                    {
                        linksDocument.Add(path, doc);

                        // Get sublinks only for attached link
                        linksDocument = GetRVTLinks(doc, linksDocument, onlyLoaded);
                    }
                }
            }

            return linksDocument;
        }

        /// <summary>
        /// Executes each rule and prepares its results to be inserted in the resulting excel
        /// </summary>
        /// <param name="document">Revit document from execute rules</param>
        /// <param name="rules">Rules to execute</param>
        /// <param name="fromLoadLinks">Indicate whether the rule is also executed on the links</param>
        /// <param name="excelPath">Excel path to store the result</param>
        /// <param name="worker">Worker to report advance</param>
        public static void Execute(Document document, List<ICheckerRule> rules, bool fromLoadLinks, string excelPath, BackgroundWorker worker)
        {
            Dictionary<ICheckerRule, DataTable> results = new Dictionary<ICheckerRule, DataTable>();

            Dictionary<string, Document> linksDocument = new Dictionary<string, Document>();

            if (fromLoadLinks)
            {
                linksDocument = GetRVTLinks(document, new Dictionary<string, Document>());
            }

            ExecutionStats.ResetInstance();

            ExecutionStats.Instance.Start = DateTime.Now;

            int step = 0;

            foreach (ICheckerRule rule in rules)
            {
                ExecutionStats.Instance.Rules.Add(string.Format("{0} ({1})", rule.Name, rule.Description));

                try
                {
                    DataTable result = rule.Execute(document);

                    ExecutionStats.Instance.Files.Add(document.IsWorkshared ? 
                        ModelPathUtils.ConvertModelPathToUserVisiblePath(document.GetWorksharingCentralModelPath()) : document.PathName);

                    foreach (KeyValuePair<string, Document> link in linksDocument)
                    {
                        result.Merge(rule.Execute(link.Value));

                        ExecutionStats.Instance.Files.Add(link.Value.IsWorkshared ?
                            ModelPathUtils.ConvertModelPathToUserVisiblePath(link.Value.GetWorksharingCentralModelPath()) : link.Value.PathName);
                    }

                    results.Add(rule, result);
                }
                catch (NotImplementedException ex)
                {
                    results.Add(rule, DataExceptionResult(rule, ex));
                }
                catch (Exception ex)
                {
                    results.Add(rule, DataExceptionResult(rule, ex));
                }
                finally
                {
                    worker.ReportProgress(100 / rules.Count * ++step);
                }
            }

            ExecutionStats.Instance.End = DateTime.Now;

            ExportExcel(results, excelPath);
        }
    }

    public class Addin
    {
        private string Id;
        private string Path;
        private Version Version;
        public List<Rule> Rules;

        public Addin(string Id, string Path, Version Version)
        {
            this.Id = Id;
            this.Path = Path;
            this.Version = Version;
            Rules = new List<Rule>();
        }

        /// <summary>
        /// Method to map Addin class to AddinsElement use in Configuration
        /// </summary>
        /// <returns>Return an instance of <c>AddinsElement</c> with map values</returns>
        public AddinsElement ToAddinsElement()
        {
            AddinsElement addin = new AddinsElement();
            addin.Id = Id;
            addin.Path = Path;
            addin.Version = Version.ToString();
            addin.Rules = new RulesCollection();

            foreach (Rule rule in Rules)
            {
                addin.Rules.Add(rule.ToRulesElement());
            }

            return addin;
        }
    }

    public class Rule
    {
        private string Id;
        private string Name;
        private string Group;
        private string Description;
        private string AssemblyQualifiedName;
        private Addin Addin;

        public Rule(string Id, string Name, string Group, string Description, string AssemblyQualifiedName, Addin Addin) {
            this.Id = Id;
            this.Name = Name;
            this.Group = Group;
            this.Description = Description;
            this.AssemblyQualifiedName = AssemblyQualifiedName;
            this.Addin = Addin;
        }

        /// <summary>
        /// Method to map Rule class to RulesElement use in Configuration
        /// </summary>
        /// <returns>Return an instance of <c>RulesElement</c> with map values</returns>
        public RulesElement ToRulesElement()
        {
            RulesElement rule = new RulesElement();
            rule.Id = Id;
            rule.Name = Name;
            rule.Group = Group;
            rule.Description = Description;
            rule.AssemblyQualifiedName = AssemblyQualifiedName;

            return rule;
        }
    }

    public class ExecutionStats
    {
        private ExecutionStats() { }

        private static ExecutionStats instance = null;

        public static ExecutionStats Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ExecutionStats();
                    instance.Rules = new NoRepeatedList<string>();
                    instance.Files = new NoRepeatedList<string>();
                }

                return instance;
            }
        }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public NoRepeatedList<string> Rules { get; set; }

        public NoRepeatedList<string> Files { get; set; }

        public static void ResetInstance()
        {
            instance = new ExecutionStats();
            instance.Rules = new NoRepeatedList<string>();
            instance.Files = new NoRepeatedList<string>();
        }
    }

    public class RepeatedRuleIdException : Exception {
        public RepeatedRuleIdException(string message) : base(message){}
    }

    public class NoRepeatedList<T> : List<T>
    {
        public new void Add(T item)
        {
            if (!Contains(item))
            {
                base.Add(item);
            }
        }
    }

    public enum LoadResultType
    {
        SUCCESS,
        ALREADY_ADDED,
        FILE_NOT_FOUND
    }

    public enum UpdateResultType
    {
        SUCCESS,
        DIFFERENT_ADDIN,
        SAME_VERSION,
        LESS_VERSION
    }
}