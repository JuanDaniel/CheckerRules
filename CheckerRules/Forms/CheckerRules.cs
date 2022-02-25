using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BBI.JD.Util;

namespace BBI.JD.Forms
{
    public partial class CheckerRules : System.Windows.Forms.Form
    {
        private UIApplication application;
        private UIDocument uiDoc;
        private Document document;

        public CheckerRules()
        {
            InitializeComponent();

            btn_Execute.Enabled = false;
        }

        public CheckerRules(UIApplication application)
        {
            InitializeComponent();

            this.application = application;
            uiDoc = application.ActiveUIDocument;
            document = uiDoc.Document;
        }

        private void CheckerRules_Load(object sender, EventArgs e)
        {
            LoadRules();
        }

        private void tree_Rules_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = node.Checked;
            }
        }

        private void btn_CheckAll_Click(object sender, EventArgs e)
        {
            CheckAll();
        }

        private void btn_Uncheck_Click(object sender, EventArgs e)
        {
            CheckAll(false);
        }

        private void btn_Execute_Click(object sender, EventArgs e)
        {
            List<ICheckerRule> rules = new List<ICheckerRule>();

            foreach (RulesElement rule in GetCheckedRules(tree_Rules.Nodes).Select(x => x.Tag).Cast<RulesElement>())
            {
                ICheckerRule instance = Core.GetInstance(rule);

                if (instance != null)
                {
                    rules.Add(instance);
                }
            }

            if (rules.Count == 0)
            {
                MessageBox.Show("You must select at least one rule before executing.", "No rules selected", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                return;
            }

            Core.Execute(document, rules, chk_Links.Checked);
        }

        private string GetTiTleForm()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            return string.Format("{0} ({1}.{2}.{3}.{4})", "Checker Rules", version.Major, version.Minor, version.Build, version.Revision);
        }

        private void LoadRules()
        {
            List<RulesElement> rules = new List<RulesElement>();

            foreach (var rc in Config.GetAddinsLoaded().Cast<AddinsElement>()
                .Select(x => x.Rules))
            {
                rules.AddRange(rc.Cast<RulesElement>());
            }

            var grouped = rules.GroupBy(x => x.Group).OrderBy(x => x.Key);

            foreach (var group in grouped)
            {
                TreeNode parent = new TreeNode(group.Key);

                foreach (var rule in group)
                {
                    TreeNode node = new TreeNode(rule.Name);
                    node.Tag = rule;
                    parent.Nodes.Add(node);
                }

                tree_Rules.Nodes.Add(parent);
            }

            tree_Rules.ExpandAll();
        }

        private void CheckAll(bool check = true)
        {
            foreach (TreeNode child in tree_Rules.Nodes)
            {
                child.Checked = false;
            }
        }

        private List<TreeNode> GetCheckedRules(TreeNodeCollection nodes)
        {
            List<TreeNode> nodesCheked = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                if (node.Tag is RulesElement && node.Checked)
                {
                    nodesCheked.Add(node);
                }

                nodesCheked.AddRange(GetCheckedRules(node.Nodes));
            }

            return nodesCheked;
        }
    }
}
