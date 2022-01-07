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
using BBI.JD.Util;

namespace BBI.JD.Forms
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadRules();
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = node.Checked;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckAll(false);
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
                    parent.Nodes.Add(new TreeNode(rule.Name));
                }

                treeView1.Nodes.Add(parent);
            }

            treeView1.ExpandAll();
        }

        private void CheckAll(bool check = true)
        {
            foreach (TreeNode child in treeView1.Nodes)
            {
                child.Checked = false;
            }
        }
    }
}
