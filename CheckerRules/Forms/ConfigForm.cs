using BBI.JD.Util;
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
using System.IO;

namespace BBI.JD.Forms
{
    public partial class ConfigForm : Form
    {
        private CheckerRules formParent;

        public ConfigForm(CheckerRules formParent)
        {
            this.formParent = formParent;

            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            LoadAddins();
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void grid_Addins_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            // Column update button
            if (e.ColumnIndex == 4)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.SomeImage.Width;
                var h = Properties.Resources.SomeImage.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(someImage, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void grid_Addins_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            // Column update button
            if (e.ColumnIndex == 4)
            {
                MessageBox.Show("Clicked!");
            }
        }

        private void LoadAddins()
        {
            grid_Addins.Rows.Clear();
            
            foreach (AddinsElement addin in Config.GetAddinsLoaded().Cast<AddinsElement>())
            {
                grid_Addins.Rows.Add(
                    addin.Path,
                    Path.GetFileNameWithoutExtension(addin.Path),
                    string.Join(", ", addin.Rules.Cast<RulesElement>().Select(x => x.Name)),
                    addin.Version
                );
            }
        }
    }
}
