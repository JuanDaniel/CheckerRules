using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            string[] groups = new string[] { "Group 1", "Group 2", "Group 3" };
            int[] rules = new int[] { 3, 2, 4, 5 };

            foreach (var group in groups)
            {
                TreeNode parent = new TreeNode(group);

                for (int i = 0; i < rules[0]; i++)
                {
                    parent.Nodes.Add(new TreeNode(string.Format("Rule {0}", i + 1)));
                }

                treeView1.Nodes.Add(parent);
            }
        }
    }
}
